using Service.Exceptions;

namespace Service;

public class ValidationHelper
{
    public static void ValidateNoDuplicates<T>(IEnumerable<T> items, Func<T, object> keySelector, string itemType) 
    {
        var duplicateKeys = items
            .GroupBy(keySelector)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateKeys.Count == 0) return;
        
        string exceptionMessage = duplicateKeys.Count == 1
            ? $"The provided {itemType} '{duplicateKeys[0]}' is duplicated. Please provide each {itemType} only once."
            : $"The following {itemType}s are duplicated: {string.Join(", ", duplicateKeys)}. Please provide each {itemType} only once.";
        throw new BadRequestException(exceptionMessage);
    }
    
    public static List<int> FilterValidIds(List<int> ids, string itemType, out List<int> invalidIds)
    {
        invalidIds = ids.Where(id => id <= 0).ToList();
        var validIds = ids.Except(invalidIds).ToList();

        return validIds;
    }
    
    public static bool ValidateItemsExistence<T>(List<int> requestedIds, List<T> foundItems, Func<T, int> idSelector, out List<int> invalidIds)
    {
        var foundIds = foundItems.Select(idSelector).ToList();
        invalidIds = requestedIds.Except(foundIds).ToList();

        return foundIds.Count != 0;
    }
}