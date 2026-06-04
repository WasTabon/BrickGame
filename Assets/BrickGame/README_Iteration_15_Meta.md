# BrickGame — Iteration 15: Daily Challenge + Achievements

## 1. Что было добавлено
- **Daily challenge** — уровень, сгенерированный по сид-числу от даты (один на день, у всех одинаковый, меняется каждый день). Кнопка DAILY в меню; отметка прохождения за сегодня.
- **Achievements** — 6 достижений с условиями, сохранение, тост «Unlocked: …» при разблокировке и экран-список (AWARDS) в меню.

## 2. Что изменилось с прошлой итерации
- `DailyChallenge` (новый) — сид от даты, `Generate(seed)`, отметка `IsTodayDone/MarkDone/GetTodayStars`.
- `Achievements` (новый) — 6 определений, `Unlock/IsUnlocked`, событие `OnUnlocked`, проверка «3 звезды на 5 уровнях».
- `GameSession` — флаг `IsDaily`.
- `LevelManager` — в daily-режиме строит уровень из сид-числа; HUD пишет «Daily Challenge».
- `DemolitionController` — теперь всегда считает броски (`ThrowsUsed`) для достижения «1 бросок».
- `BattleResultPopup` — победа в daily пишет результат дня (NEXT → меню); обычная победа триггерит достижения (first/reach10/3★×5).
- `LevelFlowController` — достижение «1 бросок на 3★».
- `ComboManager` — достижение «combo x5».
- `LevelSelectController`/`MainMenuController` — сброс/установка `IsDaily`; кнопки DAILY и AWARDS, панель достижений, тост.
- `AchievementToast` (новый) — выезжающий тост (в Game/Battle/MainMenu).
- `AchievementsPanel` (новый) — список достижений в меню.

**Достижения:**
1. First Demolition — пройти уровень 1.
2. Halfway There — открыть уровень 10.
3. Perfectionist — 3 звезды на 5 уровнях.
4. One Pull Wonder — 3 звезды одним броском.
5. Combo Master — комбо x5.
6. Daily Grinder — пройти daily.

## 3. Editor скрипты: что запустить
1. Скопируй папку `Assets/BrickGame` поверх проекта (заменит `GameSession`, `LevelManager`, `DemolitionController`, `GameHUDController`, `BattleResultPopup`, `LevelFlowController`, `ComboManager`, `LevelSelectController`, `MainMenuController`; добавит `DailyChallenge`, `Achievements`, `AchievementToast`, `AchievementsPanel`, editor ит.15).
2. Дождись компиляции.
3. В меню Unity: **BrickGame → Setup Iteration 15 (Daily + Achievements)**

Скрипт добавляет в меню кнопки DAILY и AWARDS, панель достижений и тост, а также тост в сцены Game и Battle. Идемпотентен.

## 4. Как тестировать
1. **Daily:** меню → DAILY → играется уровень дня (HUD «Daily Challenge»), пройди бой — результат запишется, кнопка станет «DAILY ✓», NEXT вернёт в меню. Повторный заход в тот же день — тот же уровень.
2. **Achievements:** меню → AWARDS — список (всё LOCKED вначале). Пройди уровень 1 → при возврате тост «Unlocked: First Demolition», в списке UNLOCKED. Собери комбо x5 → тост. И т.д.
3. Перезапусти игру — daily-отметка и достижения сохранились.

## 5. Известные ограничения / тюнинг
- Daily-уровень — процедурный из диапазонов в `DailyChallenge.Generate` (правь там вид/сложность).
- Достижения локальные (PlayerPrefs, ключи `Ach_*`, daily — `DailyDone/DailyStars`). Сброс — удалить эти ключи.
- Тост и список — простой UI; список без скролла (6 строк помещаются).
- «1 бросок» считается, т.к. броски теперь считаются всегда (даже без лимита).

## 6. Статус
Полный список из 12 пунктов реализован (10 ранее + daily + достижения). Опционально остаётся только «нефункциональная» полировка: реальный Taptic Engine, настоящие звук/арт-ассеты вместо плейсхолдеров.

---

### Тех. детали
- Сид дня: `year*10000+month*100+day`; `System.Random(seed)` → детерминированный `LevelDef`.
- Достижения через статическое событие `Achievements.OnUnlocked`; тост подписан в активной сцене.
- Скрипты без namespace и без комментариев.
