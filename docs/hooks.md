# mBMplay 모딩 및 후킹 포인트 가이드 (hooks.md)

## 1. 개요
mBMplay는 순수 C# .NET 어셈블리(exe)로 컴파일되어 있어 HarmonyX 또는 MelonLoader/Standalone Harmony를 통해 런타임 메서드 가로채기(Prefix/Postfix/Transpiler)가 매우 용이합니다. 주요 모딩 후보 영역과 Harmony 패치 예시를 기술합니다.

---

## 2. 주요 후킹 포인트 후보

### (1) 판정 조작 (All-Perfect / Custom Judge Window Mod)
- **타겟 1**: `MBMPlay.PlayScene.LoadBms()`
  - **시그니처**: `private void LoadBms()`
  - **설명**: BMS 로드 시 판정 범위 객체 `_judgeRange` (`PGreat`, `Great`, `Good`, `Bad` 등)를 결정합니다. 이 값을 크게 확장하면 All-PGreat가 가능합니다.
- **타겟 2**: `MBMPlay.Numbers.SetJudge(int lane, JudgeType judge, double time)`
  - **시그니처**: `public static void SetJudge(int lane, JudgeType judge, double time)`
  - **설명**: 노트 타격 시 최종 판정 카운트 및 게이지를 업데이트하는 핵심 관문입니다. 파라미터 `judge`를 `JudgeType.PGreat`로 강제 변경(Prefix)할 수 있습니다.

### (2) 차트/배치 커스텀 주입 (Custom Chart / BMS Injection)
- **타겟**: `BeMusic.Bms.Parse(string filePath, int defaultJudgeRank, XorShift random)`
  - **시그니처**: `public static Bms Parse(string filePath, int defaultJudgeRank, XorShift random)`
  - **설명**: 디스크의 `.bms` 파일을 파싱하여 `Bms` 객체를 반환합니다. Postfix에서 커스텀 노트 오브젝트를 추가하거나 BMS 메타데이터(제목, 아티스트, 난이도 등)를 동적으로 교체할 수 있습니다.

### (3) 레인 배치 및 랜덤 규칙 커스텀 (Lane Swap Mod)
- **타겟**: `BeMusic.BmsTimeTable.SwapLanes(int keys, LaneSwapOption player1SwapOption, LaneSwapOption player2SwapOption, bool flip, long randomSeed)`
  - **설명**: 플레이어의 미러, 랜덤, S랜덤, 플립 등 레인 배치를 변형하는 메서드입니다. 사용자 정의 랜덤 알고리즘이나 고정 배치를 주입할 수 있습니다.

### (4) 자동 연주 및 입력 시뮬레이션 (Auto-Play / Bot Mod)
- **타겟**: `MBMPlay.Flags.AutoPlay` (Property)
  - **타겟 메서드**: `MBMPlay.PlayScene.Update(double time)`
  - **설명**: AutoPlay 플래그를 강제로 활성화하거나 키 입력 이벤트 핸들러(`PlayingKeyState`)를 후킹하여 원하는 타이밍에 입력을 주입할 수 있습니다.

---

## 3. Harmony 패치 코드 예시

### 예시 1: All-Perfect (All P-Great) 판정 강제 모드
`Numbers.SetJudge` 메서드의 첫 번째 인자 `judge`를 무조건 `JudgeType.PGreat`로 덮어쓰는 Prefix 패치:

```csharp
using HarmonyLib;
using MBMPlay;

[HarmonyPatch(typeof(Numbers), nameof(Numbers.SetJudge))]
public static class AllPerfectPatch
{
    // __0: lane, __1: judge, __2: time
    [HarmonyPrefix]
    public static void Prefix(ref JudgeType judge, ref double time)
    {
        if (judge != JudgeType.None)
        {
            // 모든 유효 입력을 PGREAT (0.0ms 오차)로 보정
            judge = JudgeType.PGreat;
            time = 0.0;
        }
    }
}
```

### 예시 2: BMS 파라미터 (All-Perfect 유도용 판정 범위 확장) 모드
`PlayScene.LoadBms` 완료 직후 Reflection을 통해 `_judgeRange`의 `PGreat` 범위를 극대화하는 Postfix 패치:

```csharp
using System.Reflection;
using HarmonyLib;
using MBMPlay;

[HarmonyPatch(typeof(PlayScene), "LoadBms")]
public static class JudgeRangeEnlargerPatch
{
    private static readonly FieldInfo JudgeRangeField = 
        typeof(PlayScene).GetField("_judgeRange", BindingFlags.Instance | BindingFlags.NonPublic);

    [HarmonyPostfix]
    public static void Postfix(PlayScene __instance)
    {
        if (JudgeRangeField == null) return;

        object judgeRangeObj = JudgeRangeField.GetValue(__instance);
        if (judgeRangeObj is JudgeRange judgeRange)
        {
            // PGREAT 범위를 500ms(전체 구간)로 확장하여 거의 모든 타격이 PGREAT가 되도록 조작
            judgeRange.PGreat = 500.0;
            judgeRange.Great = 500.0;
            judgeRange.Good = 500.0;
            judgeRange.Bad = 500.0;
            judgeRange.FastPoor = 500.0;
            judgeRange.SlowPoor = -500.0;
        }
    }
}
```

### 예시 3: 곡 메타데이터 (제목/아티스트) 실시간 변조 모드
```csharp
using HarmonyLib;
using BeMusic;
using Randomize;

[HarmonyPatch(typeof(Bms), nameof(Bms.Parse), new[] { typeof(string), typeof(int), typeof(XorShift) })]
public static class CustomTitlePatch
{
    [HarmonyPostfix]
    public static void Postfix(Bms __result)
    {
        if (__result != null)
        {
            __result.Title = "[MOD] " + __result.Title;
            __result.Artist = "Custom Artist by 화영왕";
        }
    }
}
```
