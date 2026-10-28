-- Заполнение релейшена оборудования полями IsDraft и EquipmentType из таблицы событий
--
-- Зачем: документ целиком лежит в jsonb, а популярные для фильтрации поля дублируются в релейшен -
-- индексы висят на нём, а в документ идём уже по найденному EntityId
-- Когда поле добавили в события, в релейшене у старых записей его нет - скрипт переносит его
-- из последней версии события каждой сущности (удалённые пропускаются)
-- Запускается при деплое на любой стенд
--
-- Скрипт из исходной системы, в схему этого фрагмента не входит: таблицы "Events" и "Equipments"
-- живут в сервисе записи, данные тянутся из его таблицы событий в его релейшен
-- Аналог в этом фрагменте - EquipmentDocuments -> EquipmentIndex

with "Events" as (
    select
        e."EntityId",
        e."Data"::jsonb AS "Data"
    from "Events" as e
             inner join
         (
             select "EntityId", max("Version") as "Version"
             from "Events" te
             group by "EntityId"
         ) as t
         on e."EntityId"=t."EntityId" and e."Version"=t."Version"
    where e."EventType" <> 2
)
update "Equipments" as eq
set
    "EquipmentType" = (ae."Data"->>'equipmentType')::int,
  "IsDraft" = (ae."Data"->>'isDraft')::boolean
from "Events" ae
where eq."EntityId" = ae."EntityId";
