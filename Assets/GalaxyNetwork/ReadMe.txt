/* =================================
 * GGTeam © GalaxyNetwork free Asset
 * ---------------------------------
 * All right reserved 2017-2018
 * Murnik Roman, Totmin Deyn
 * =================================
*/


Струкрура папок GalaxyNetwork
-----------------------------

Assemblies				- Библиотека сетевого движка. Наша гордость)
Core					- Файлы ядра фреймворка
	Components			- Основные сетевые компоненты
	HelpsAttributes		- Дополнительные файлы для описания проекта
editor					- Файлы для меню и визуализации редактора
modules_canvas			- Модули (Окна) помогающие в быстрой разработке
Resources				- Файлы ресурсов (шрифтов и текстур)


-----------------------------

Changed:
snapThresholdPosition -> thresholdPosition
snapThresholdRotation -> thresholdRotation
offInterpolationForInvisible(Не интерполировать, невидимый объект) Временно отключен и передан на доработку.
Создан второй тип интерполяции, более плавный для медленных перемещений. Предыдущая интерполяция включается сама при speedInterpolationPosition = 0