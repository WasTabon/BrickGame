# BrickGame — Iteration 19: URP + Post-Processing

Это переход на URP 2D и «ААА-лук» (Bloom / Vignette / Chromatic Aberration / Color Grading). Часть шагов — ручные (установка URP и назначение пайплайна; автоматизировать это безопасно нельзя). Скрипт пост-обработки **по умолчанию выключен** и ничего не ломает, пока ты сам не включишь его символом.

## ВАЖНО: порядок действий
Скрипт `BrickGameSetup_Iteration19_URP.cs` обёрнут в `#if BRICKGAME_URP` — пока символа нет, файл «пустой» и безопасен. Сначала ставим URP, потом включаем символ.

---

## Шаг 1. Установить URP
1. **Window → Package Manager → Unity Registry → найти «Universal RP» → Install.**

## Шаг 2. Создать URP Asset с 2D Renderer
1. В Project: **Create → Rendering → URP Asset (with 2D Renderer)**.
   - Создадутся два ассета: `…_PipelineAsset` и `…_Renderer2D` (2D Renderer Data). Положи их, например, в `Assets/BrickGame/Rendering/`.
   - Если такого пункта нет — создай **URP Asset (with Universal Renderer)**, затем отдельно **Create → Rendering → 2D Renderer**, и в Pipeline Asset → Renderer List подставь 2D Renderer.

## Шаг 3. Назначить пайплайн
1. **Project Settings → Graphics → Scriptable Render Pipeline Settings** → перетащи свой `…_PipelineAsset`.
2. **Project Settings → Quality** → для текущего уровня тоже выставь этот Pipeline Asset.
3. Проверь сцену Game/Battle: спрайты должны рендериться как обычно (для 2D дефолтные спрайт-материалы работают под URP без конвертации). Если что-то стало «маджента» — **Edit → Rendering → Materials → Convert All Built-in Materials to URP**.

## Шаг 4. Включить авто-настройку пост-обработки
1. **Project Settings → Player → Other Settings → Scripting Define Symbols** → добавь:
   ```
   BRICKGAME_URP
   ```
   (Применить/Apply — проект перекомпилируется, скрипт «оживёт».)
2. В меню Unity появится пункт: **BrickGame → Setup URP Post FX** — запусти его.
   - Создаст профиль `Assets/BrickGame/Rendering/BrickGamePostFX.asset` с Bloom / Vignette / Chromatic Aberration / Color Adjustments.
   - Добавит Global Volume во все сцены и включит Post Processing на камере.

## Что получишь
- **Bloom** — светятся яркие места: звёзды, вспышки выстрелов, взрывы, конфетти, акцентные (оранжевые) элементы.
- **Vignette** — мягкое затемнение краёв (теперь «настоящая», URP-овая; UI-виньетку из ит.18 можешь удалить).
- **Chromatic Aberration** — лёгкое цветовое расхождение по краям (киношность).
- **Color Adjustments** — чуть больше насыщенности/контраста.

## Тюнинг
Открой `BrickGamePostFX.asset` (или Global Volume) в инспекторе и крути значения: `Bloom.intensity/threshold`, `Vignette.intensity`, `ChromaticAberration.intensity`, `ColorAdjustments`. Изменения сразу видны в Play.

## Про настоящий 2D-свет (свечение бомб/льда, rim-light, тени)
Это отдельный слой поверх URP 2D и требует, чтобы спрайты использовали **Sprite-Lit-Default** материал (иначе Light2D на них не влияет):
1. Выдели спрайты/префабы → материал `Sprite-Lit-Default`.
2. Добавь **Global Light 2D** (GameObject → Light → 2D → Global Light) — базовая освещённость.
3. Точечные **Light2D (Point)** на бомбах/льде для свечения; на башню — directional/point для rim-эффекта; тени включаются на Light2D + Shadow Caster 2D на блоках.

Скажи, когда сделаешь Шаги 1-3 (URP встанет) — и я отдельной итерацией добавлю скриптом: перевод нужных спрайтов на Sprite-Lit, Global Light 2D и точечные источники на бомбах/льде (через `#if BRICKGAME_URP`, так же безопасно).

## Откат
- Убери `BRICKGAME_URP` из Scripting Define Symbols — скрипт снова станет инертным.
- В Graphics/Quality верни пустой Pipeline (Built-in) — вернётся прежний рендер.

---

### Тех. детали
- Скрипт целиком под `#if BRICKGAME_URP`, поэтому без символа не компилируется и не влияет на проект.
- Профиль создаётся через `VolumeProfile` + `Add<Bloom/Vignette/ChromaticAberration/ColorAdjustments>()`; камера — `GetUniversalAdditionalCameraData().renderPostProcessing = true`.
- Скрипты без namespace и без комментариев.
