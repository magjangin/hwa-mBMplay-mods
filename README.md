# hwa-mBMplay-mods

mBMplay v3.24.0824.1 (x64)을 디컴파일해 복원한 소스에 한국어화와 호환성 개선을 더한 연구용 저장소입니다.

## 구성

- `src/` — ilspycmd로 복원한 C# 프로젝트 (.NET Framework 4.7.2, x64)
- `src/bin/Debug/net472/skin/` — 한국어화한 기본 스킨
- `input/` — 빌드에 필요한 원본 DLL과 원본 실행 파일
- `docs/` — 구조 분석, 후킹 포인트 정리
- `test_bms/` — 파서 호환성 테스트용 BMS
- `PROGRESS.md` — 단계별 작업 기록

## 빌드

```
dotnet build src/mBMplay.csproj
```

결과물은 `src/bin/Debug/net472/`에 생깁니다. 실행하려면 원본 배포본의 나머지 파일(`dll/`, `settings/`, `mBMconfig.exe` 등)을 같은 폴더에 복사하세요.

## 라이선스

mBMplay는 misty.ls04의 저작물이며 mBMplay Licence v1.0을 따릅니다(비상업 이용 한정). 이 저장소의 수정본도 같은 라이선스로 배포합니다.

- [license.txt](license.txt) — mBMplay Licence v1.0 전문 (원문, 일본어)
- [license_ko.txt](license_ko.txt) — 한국어 참고 번역 (비공식, 원문이 우선)
- [license_library.txt](license_library.txt) — 포함된 라이브러리들의 라이선스 (영어 원문)
