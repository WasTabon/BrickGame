# BrickGame — Iteration 12: Progress + Level Select

## 1. Что было добавлено
- Сохранение прогресса: открытый максимум уровня и лучшие звёзды на каждом (PlayerPrefs).
- Экран выбора уровня (карта 2×3): номер, заработанные звёзды, замок на неоткрытых.
- Позиции ям вынесены в инспектор `LevelManager` (`pitCentersSingle`, `pitCentersDouble`).

## 2. Что изменилось с прошлой итерации
- `SaveSystem` — `UnlockedLevel`, `GetStars/SetStars`, `CompleteLevel`.
- `GameSession` — добавлено `LastStars`.
- `LevelFlowController` — запоминает звёзды демо-фазы в `GameSession.LastStars`.
- `BattleResultPopup` — при победе: `SaveSystem.CompleteLevel(уровень, звёзды)` (фиксирует прохождение + открывает следующий).
- `MainMenuController` — кнопка PLAY теперь открывает сцену `LevelSelect` (а не сразу Game).
- `LevelManager` — позиции ям из инспектора (`pitCentersSingle`/`pitCentersDouble`), с безопасным фолбэком.
- Новая сцена `LevelSelect` + добавление в Build Settings.
- Прочий тюнинг/логика прошлых итераций сохранены.

## 3. Editor скрипты: что запустить
1. Скопируй папку `Assets/BrickGame` поверх проекта (заменит `LevelManager`, `GameSession`, `LevelFlowController`, `BattleResultPopup`, `MainMenuController`; добавит `SaveSystem`, `LevelSelectController`, editor ит.12).
2. Дождись компиляции.
3. В меню Unity: **BrickGame → Setup Iteration 12 (Progress + Level Select)**

Скрипт прописывает массивы позиций ям в `LevelManager` (Game-сцена), строит сцену `LevelSelect` и добавляет её в Build Settings.

## 4. Как тестировать
1. Из главного меню нажми PLAY → откроется карта уровней (открыт только 1-й, остальные с замком).
2. Тапни уровень 1 → демо-фаза → COLLECT → BATTLE → победа.
3. После победы вернись (NEXT ведёт на следующий уровень) — на карте теперь открыт уровень 2, а на уровне 1 горят заработанные звёзды.
4. Перезапусти игру — прогресс и звёзды сохранились.

> Позиции ям: выдели `LevelManager` в Game-сцене → поля `Pit Centers Single` (1 яма) и `Pit Centers Double` (2 ямы). Меняй X-центры прямо в инспекторе.
> Для теста прогресса с нуля удали PlayerPrefs (или ключи `UnlockedLevel`, `Stars_*`).

## 5. Ожидаемый результат
- Карта 2×3 со звёздами и замками; недоступные уровни не нажимаются.
- Прохождение боя открывает следующий уровень и пишет звёзды демо-фазы.
- Прогресс переживает перезапуск.

## 6. Известные ограничения текущей итерации
- Звёзды на карте = звёзды демо-фазы того прохождения, где была победа в бою (проигрыш в бою уровень не засчитывает).
- 6 ячеек на экране без скролла (под текущее число уровней). Больше уровней → понадобится скролл/страницы.
- Сейв локальный (PlayerPrefs), облака нет.

## 7. Что будет в следующей итерации
Пазл-слой: движущаяся яма, лимит бросков на уровень, хрупкие/липкие блоки.

---

### Тех. детали
- `SaveSystem` — обёртка над PlayerPrefs (`UnlockedLevel`, `Stars_<n>`).
- Прохождение фиксируется в `BattleResultPopup` при победе (уровень + `GameSession.LastStars`).
- `LevelSelect` — отдельная сцена с менеджерами; кнопки строит editor, состояние заполняет `LevelSelectController` в Start.
- Скрипты без namespace и без комментариев.
