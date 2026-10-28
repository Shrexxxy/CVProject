using Contracts;
using Contracts.Equipment;
using BL.Abstractions;
using BL.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Оборудование организации
/// </summary>
[ApiController]
[Route("api/equipments")]
public class EquipmentController(IEquipmentService equipmentService) : ControllerBase
{
    /// <summary>
    /// Возвращает страницу списка оборудования по фильтру
    /// </summary>
    /// <remarks>
    /// Доступные поля для сортировки: BuildingName, Name, ManufacturerName, ReleaseDate,
    /// InputExploitationDate, Count. По умолчанию - дата последнего обновления
    /// </remarks>
    /// <param name="filter">Фильтр, сортировка и пагинация</param>
    /// <param name="validator">Валидатор фильтра</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="200">Страница списка</response>
    /// <response code="400">Ошибки валидации фильтра</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PagingResult<EquipmentRecordItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetItems([FromBody] EquipmentFilterParameters filter,
        [FromServices] IValidator<EquipmentFilterParameters> validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(filter, cancellationToken);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        return Ok(await equipmentService.GetItems(filter, cancellationToken));
    }

    /// <summary>
    /// Возвращает карточку оборудования
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="200">Карточка оборудования</response>
    /// <response code="404">Документ не найден или удалён</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EquipmentModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var equipment = await equipmentService.GetById(id, cancellationToken);

        return equipment is null ? NotFound() : Ok(equipment);
    }

    /// <summary>
    /// Создаёт карточку оборудования
    /// </summary>
    /// <param name="parameters">Параметры карточки</param>
    /// <param name="validator">Валидатор карточки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="201">Документ создан</response>
    /// <response code="400">Ошибки валидации</response>
    [HttpPost]
    [ProducesResponseType(typeof(DocumentMeta), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EquipmentParameters parameters,
        [FromServices] IValidator<EquipmentParameters> validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(parameters, cancellationToken);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        var created = await equipmentService.Create(parameters);

        return CreatedAtAction(nameof(GetById), new { id = created.EntityId }, created);
    }

    /// <summary>
    /// Изменяет карточку оборудования
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <param name="parameters">Параметры карточки и версия, которую видел клиент</param>
    /// <param name="validator">Валидатор изменения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="200">Документ сохранён</response>
    /// <response code="400">Ошибки валидации</response>
    /// <response code="404">Документ не найден или удалён</response>
    /// <response code="409">Документ уже изменён другим пользователем</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DocumentMeta), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EquipmentUpdateParameters parameters,
        [FromServices] IValidator<EquipmentUpdateParameters> validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(parameters, cancellationToken);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        return ToActionResult(await equipmentService.Update(id, parameters));
    }

    /// <summary>
    /// Помечает карточку оборудования удалённой
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <response code="200">Документ помечен удалённым</response>
    /// <response code="404">Документ не найден или уже удалён</response>
    /// <response code="409">Документ изменён во время удаления</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DocumentMeta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return ToActionResult(await equipmentService.Delete(id));
    }

    private IActionResult ToActionResult(DocumentWriteResult result)
    {
        return result.Status switch
        {
            DocumentWriteStatus.Ok => Ok(result.Document),
            DocumentWriteStatus.NotFound => NotFound(),
            DocumentWriteStatus.VersionConflict => Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: "Документ уже изменён другим пользователем. Обновите карточку и повторите сохранение."),
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }
}
