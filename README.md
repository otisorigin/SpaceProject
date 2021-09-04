# SpaceProject

Pet-проект по реализации простой пошаговой стратегии в духе XCOM на Unity.

Build на WebGL: https://otisorigin.github.io/SpaceProject-WebGL/

Trello доска проекта: https://trello.com/b/jxXAc7aS/space-project

Исходный код проекта: https://github.com/otisorigin/SpaceProject/tree/master/Assets/Game/Scripts

Ключевые особенности:
 - Графика 2D на спрайтах, вид сверху
 - 2 игрока, ходят по очереди
 - Поле симметричных размеров
 - 3 размера юнитов (по занимаемым клеткам) 1х1, 2х2, 3х3
 - 6 видов юнитов
 
Что реализовано: 
   - Отображение сетки поля
   - Динамически генерируемый бэкграунд перед боем (на фоне космоса динамически расставляются туманности и планеты)
   - Динамически генерируемые препятствия на карте (астероеды) перед боем
   - Подсветка юнитов при наведении курсора: синий (можно выбрать этот юнит), зеленый (выбранный юнит), красный (вражеский юнит)
   - Выбор юнита для передвижения курсором с помощью мыши
   - Передвижение камеры по карте клавишами W,A,S,D
   - При выборе юнита, доступная область хода обозначается желтым;
   - У каждого юнита есть своя дальность хода
   - Выбор траектории движения осуществляется передвижением курсора с помощью мыши
   - При выборе пути траектория движения отображается на карте
   - У игрока есть возможность не тратить сразу весь ход юнита, а пройти часть пути и затем переключится на другого юнита
   - Если нет необходимости ходить юнитом или проходить всю возможную длину пути, можно задать ему режим обороны
   - Есть возможность сбросить установленный путь в процессе движения юнита (кнопка Reset Path)
   - Для выбора следующего доступного юнита есть кнопка (Next Unit)
   - Завершение хода и переключение на следующего игрока
   - Компонент здоровья и полоска HP у каждого юнита
   - 6 видов юнитов (без настроенных характеристик)

Что осталось реализовать:
   - Режим стрельбы
   - Меню, интерфейсы
   - Выбор комбинации юнитов из 6 штук перед боем каждым игроком
   - Графические эффекты (эффект реактивных двигателей, эффекты стрельбы и т.д.)
 
 Технические аспекты:
 - Pathfinding на алгоритме A*
 - Сетки графов для юнитов трех размеров (1х1, 2х2, 3х3)
 - Подключен DI контейнер Zenject
 - 4 GameState: Выбор юнита (UnitSelection), Движение юнита (UnitMovement), Стрельба юнитом (UnitAttack), Конец хода игрока (EndOfTurn)
 - При расчете пути на сетке графов учитываются статические препятствия (астероиды) и динамические препятствия (другие юниты)
 - Старался по максимуму переносить логику из Update методов (вызываются на каждом Tick-е) на эвенты

Использованные ресурсы:
 - Видеоурок Pathfinding 1x1 по алгоритму Дийкстры: https://www.youtube.com/watch?v=kYeTW2Zr8NA
 - Видеоурок Unity Healthbar: https://www.youtube.com/watch?v=CA2snUe7ARM
 - Ассеты: https://opengameart.org/content/space-game-art-pack-extended
 
