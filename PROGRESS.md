# mBMplay .NET 어셈블리 복원 및 분석 진행 현황

- 대상 파일: `H:\mbmplay_3240824_1_x64\mBMplay\mBMplay.exe`
- 작업 디렉터리: `H:\source\repos\mBMplay-dotnet`

## 단계별 상태
- [완료] 1단계: C# 프로젝트 디컴파일 — `H:\source\repos\mBMplay-dotnet\src` — ilspycmd를 통해 프로젝트 전체 복원 완료 (`SwapLanes` 디컴파일 예외는 바디 복원으로 보완)
- [완료] 2단계: 구조 파악 — `H:\source\repos\mBMplay-dotnet\docs\analysis.md` — 네임스페이스 계층, 진입점, 의존 라이브러리, 데이터 구조 상세 정리 완료
- [완료] 3단계: 빌드 검증 — `H:\source\repos\mBMplay-dotnet\src\bin\Debug\net472\mBMplay.exe` — 참조 충돌 및 리소스 직렬화 오류 해결 후 `dotnet build` 0 Error 성공
- [완료] 4단계: 후킹 포인트 정리 — `H:\source\repos\mBMplay-dotnet\docs\hooks.md` — 판정 조작, BMS 주입, Harmony 패치 예시 작성 완료
- [완료] 5단계: bms editer 호환 파싱 확장 — `H:\source\repos\mBMplay-dotnet\src\BeMusic\Bms.cs`, `H:\source\repos\mBMplay-dotnet\src\MBMPlay\PlayScene.cs` — BOM 없는 UTF-8 엄격 감지, 최소 헤더 인식 완화, 3자리 키 인덱스/슬롯 지원, 오디오 대소문자 및 재귀 경로 탐색 구현, `dotnet build` 빌드 검증 및 단위 테스트 완료
- [완료] 6단계: UI 한국어화 — `PlatformGeneral.cs`, `PlayScene.cs`, `Program.cs` 다이얼로그/창 타이틀 한국어 적용 및 `load.png`(시작/로딩 화면) 한국어 텍스처 리터칭 완료
- [완료] 7단계: 인게임 정보 패널 한국어화 — `common_bg.png` 텍스처 하단 영역에 고해상도 한국어 라벨(레벨, 판정, 난이도별 판정값 EASY/NORMAL/HARD/V.HARD, 노트, 최대 콤보, 초당 노트, 최대, 평균, 최소, 총합, 게이지, 마디, 롱노트, 미스, 빠름, 느림, HS 버튼 라벨 등) 렌더링 및 `side_info.xml`, `bottom_info.xml` 매핑 완료
- [완료] 8단계: 단독 음원(BGM) 자동 감지 및 재생 확장 — 뮤즈대시 커스텀 등 `#xxx01` BGM 채널 노트가 생략되고 `#WAV0HS music.ogg` 또는 폴더 내 `music.*`/`bgm.*` 파일만 존재하는 차트의 경우, 마디 0(시간 0.0s)에 BGM 채널 이벤트로 자동 연동하여 재생되도록 `PlayScene.LoadBms()` 확장 구현 및 빌드 검증 완료
- [완료] 9단계: 식스타 게이트 커스텀 비디오(MP4 BGA) 자동 연동 및 Media Foundation 고속 디코더 구현 — 식스타 게이트 커스텀 차트처럼 `#BMP` 및 BGA 채널 이벤트가 생략된 경우 같은 폴더의 `video.mp4`/`bga.mp4`를 자동 감지해 마디 0에 BGA 이벤트로 주입하고, 64비트 Windows DirectShow MP4 디코딩 한계를 극복하기 위해 Windows 내장 Media Foundation 기반 `MediaFoundationMovie`(NV12 하드웨어 가속 디코딩 -> BGRA32 실시간 변환 및 오디오 싱크 제어)를 구현하여 인게임 BGA 재생 및 렌더링 검증 완료
- [완료] 10단계: 동영상 BGA 상단 초록 띠 버그 수정 및 BGA 코드 리팩토링 — `src\MBMPlay\MediaFoundationMovie.cs`, `src\MBMPlay\PlayScene.cs` — 원인: H.264 디코더가 첫 샘플에서 출력 타입을 1920x1088(16 정렬)로 바꾸는데 NV12 변환이 1080 기준으로 UV 평면 위치를 계산해 위 16줄이 초록(U=V=0)으로 나오고 색차가 16px 밀림. 디코더 실제 출력 타입(정렬 높이·stride·표시 영역)을 `CurrentMediaTypeChanged`마다 다시 읽도록 수정. 함께 고친 것: `MF_PD_DURATION` GUID 오류로 `TotalTime`이 항상 0이던 문제(재시작 시 영상 위치가 0으로 감), 디코딩 즉시 내보내던 프레임을 타임스탬프 기준으로 내보내고 늦은 프레임은 건너뛰도록(음악과 싱크 유지), BT.709/601 색 행렬·레인지 자동 선택, NV12→BGRA 변환 병렬화(Debug 빌드 기준 ~15fps → 24fps 전량), `SetStreamSelection` 인덱스 오류, Stop/Play/Pause 의미를 DirectShowMovie와 통일. `PlayScene`은 대체 BGA 주입을 `AddFallbackBgaMovie()`로 분리, 하드코딩된 `bga_debug.log` 기록 제거, 동영상 재시작/재생/그리기 중복 코드를 `RestartMovie`/`PlayMovie`/`TryGetBgaSize`로 정리. 실제 `video.mp4`로 디코딩 하네스 검증(상단 행 색 정상, 3초간 72/72 프레임, TotalTime 294.5s, Stop→되감기, 200s 위치 재시작)
- [완료] 11단계: 600줄 넘는 파일 분할(1차, 동작 변경 없음) — 11개 파일을 책임별 `partial` 파일로 나눔(예: `PlayScene.Update/Input/Loading/Bga/Draw/Commands/Restart.cs`, `Flags.Dictionary*.cs`, `Numbers.Options/Judge/Dictionary*.cs`, `Bms.Parse/File/Regex.cs`). ILSpy가 4줄로 펼친 스킨 값 사전은 한 줄 항목으로 접고, Flags(786개)·Numbers(706개)는 분류별 `Add*` 메서드로 나눔. 초기화식이 있는 필드는 원래 파일·순서에 남겨 초기화 순서를 보존. 검증: `main` 빌드와 디컴파일 결과를 타입별로 대조해 멤버 동일(순서만 다름), 사전 키·람다 쌍 동일, 초기화 필드 순서 동일. 남은 600줄 초과: 거대 메서드 3개(`Skin.ReadSkinBody` 1843줄, `PlayScene.Update` 1604줄, `Program.Main` 706줄) — 2단계에서 메서드 분해 예정
