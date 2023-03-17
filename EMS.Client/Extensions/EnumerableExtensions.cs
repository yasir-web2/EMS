using EMS.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Client.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<SelectListItem> ToSelectListItems(this IEnumerable<DropdownResponse>? items, Guid? selectedId = null)
    {
        if (items is null || !items.Any())
            return Enumerable.Empty<SelectListItem>();

        return items.OrderBy(item => item.Name)
                    .Select(item =>
                    new SelectListItem
                    {
                        Selected = item.Id == (selectedId ?? Guid.Empty),
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
    }
}