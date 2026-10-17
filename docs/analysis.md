# mBMplay 구조 분석 보고서 (analysis.md)

## 1. 개요
- **대상 바이너리**: `mBMplay.exe` (v3.24.824.1, x64, .NET Framework 4.7.2)
- **주요 용도**: DirectX9/11/12, OpenGL, Vulkan 다중 렌더링 백엔드 및 WASAPI/XAudio2/DirectSound/OpenAL 멀티 오디오 백엔드를 지원하는 고성능 BMS(Be-Music Source) 플레이어.
- **프로젝트 위치**: `H:\source\repos\mBMplay-dotnet\src`

---

## 2. 진입점 및 실행 흐름

### 진입점 (Entry Point)
- **클래스**: `MBMPlay.Program`
- **메서드**: `private static void Main(string[] args)`
- **특징**: `[STAThread]` 진입점, 단일 인스턴스 뮤텍스(`ExclusiveProcess`) 및 IPC 파이프(`IpcServer<RemoteProxy>`)를 통해 중복 실행 방지 및 외부 인자 수신 지원.

### 실행 파이프라인
```
[Main(args)]
   │
   ├─► CLI 인자 파싱 (/S, /T, /RANMIR, /FLIP, /GR, /AU, /VSYNC, /UPS 등)
   ├─► 중복 실행 검사 (ExclusiveProcess) 및 IPC 서버 시작
   ├─► 설정 로드 (mBMplay.xml / KeyConfig.xml)
   ├─► 그래픽스 엔진 초기화 (PlatformGeneral.CreateGraphics)
   ├─► 메인 윈도우 생성 (GraphicsWindow)
   ├─► 오디오 엔진 초기화 (PlatformGeneral.CreateAudioInstance)
   ├─► 입력 장치 초기화 (InputDevices.Init, DirectInput)
   ├─► 씬(Scene) 생성 및 SceneManager 등록
   │     ├─ 인자 bms 파일 존재: PlayScene 생성
   │     └─ 미지정: PreloadScene 생성
   └─► 메인 루프 (Update / Render) 진입
```

---

## 3. 네임스페이스 및 클래스 계층 구조

| 네임스페이스 | 주요 클래스 | 역할 및 기능 |
|:---|:---|:---|
| `BeMusic` | `Bms`, `BmsTimeTable`, `BmsTimeTableRow`, `BmsLaneTime`, `BmsBar`, `BmsObject` | BMS/BME/PMS 파일 파싱, 오브젝트 배치 테이블, 틱/시간 변환, 레인 스왑(랜덤/미러/S랜덤 등) 알고리즘 |
| `BeMusic.Extentions` | `FloatExtensions`, `DictionaryExtensions` | 수학적 부동소수점 처리 및 컬렉션 확장 메서드 |
| `MBMPlay` | `Program`, `SceneManager`, `Scene`, `PlayScene`, `PreloadScene` | 메인 라이프사이클 관리, 씬 제어, 게임플레이 코어 |
| `MBMPlay` (UI/Skin) | `Skin`, `ISkinItem`, `SingleDestinationSkinItem`, `MultipleDestinationSkinItem`, `NumberSkinItemBase`, `Font`, `DirectShowMovie` | LR2/mBMplay 스킨 CSV/DST 파싱, 2D 텍스처 애니메이션, BGA 재생, 폰트 렌더링 |
| `MBMPlay` (State) | `Flags`, `Numbers`, `Texts`, `EventTimers`, `PlayingKeyState`, `AutoPlayKeyState` | 게임플레이 전역 상태, 판정 카운트/점수, 이벤트 타이머 관리 |
| `MBMPlay.Common` | `Settings`, `KeyConfigRoot`, `AudioSettings`, `GraphicsSettings`, `SkinInfo`, `SkinSettings` | XML 직렬화 기반 프로그램 환경설정, 키설정, 스킨 메타데이터 |
| `MBMPlay.Platform` | `PlatformGeneral`, `ExclusiveProcess`, `Interop` | 그래픽/오디오 팩토리, 윈도우 타이머 정밀도(timeBeginPeriod), 네이티브 API 호출 |
| `MBMPlay.Platform.Ipc` | `IpcServer<T>`, `IpcClient<T>`, `RemoteContainer` | IPC(Remoting) 기반 외부 프로세스 통신 (BMS 파일 드롭 전달 등) |
| `Randomize` | `XorShift` | 고속 난수 생성기 (시드 기반 결정론적 난수 생성) |

---

## 4. 외부 의존성 (Dependencies)

### 자체/커스텀 엔진 DLL (`input/dll/`)
- `MistGraphics.dll`, `MistGraphics.*.dll`: Mist 엔진 렌더러 프론트엔드 및 백엔드 (DirectX9, DirectX11, DirectX12, OpenGL, Vulkan)
- `MistAudio.dll`, `MistAudio.*.dll`: Mist 엔진 오디오 프론트엔드 및 백엔드 (XAudio2, Wasapi, DirectSound, OpenAL)
- `MistInput.dll`, `MistInput.DirectInput.dll`: 키보드, 마우스, 조이스틱 입력 장치 추상화
- `MistUtils.dll`, `MistUtils.SpanUtils.dll`: 수학 연산, 메모리 버퍼, 문자열 처리 유틸리티
- `MafuFont.dll`: 벡터/비트맵 텍스트 렌더링
- `Tsukikage.DirectShow.dll`: DirectShow 기반 비디오/동영상 BGA 디코더
- `UnmanagedArray.dll`: 고속 비관리 힙 메모리 배열 지원

### 외부 서드파티 라이브러리
- `SharpDX.*.dll`: DirectX 런타임 래퍼
- `SkiaSharp.dll` / `libSkiaSharp.dll`: 2D 그래픽 및 이미지 디코딩
- `NVorbis.dll` / `CSCore.dll`: OGG Vorbis 및 오디오 스트림 디코딩
- `OpenTK.dll`: OpenGL 바인딩
- `Vulkan.dll`: Vulkan 그래픽 파이프라인

---

## 5. 핵심 데이터 구조 및 BMS 파싱 특이사항
- **BMS Time Formula**: `Bms.cs` 및 `BmsTimeTable.cs`에서 틱(Tick)과 4분음표 비트 단위를 바탕으로 시간(ms)을 계산.
- **판정 시스템**:
  - `PGreat`: 18.0ms (Normal 기준)
  - `Great`: 40.0ms
  - `Good`: 100.0ms
  - `Bad`: 200.0ms
  - `FastPoor`: 500.0ms
  - `SlowPoor`: -200.0ms
  - 스크래치 레인 판정 범위 배율: 1.1x ~ 1.2x
  - 롱노트 끝 판정 배율: 1.2x ~ 1.5x
