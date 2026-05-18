using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public static class GetOverlappingSuppliersWithRatesQueryHandler
{
  public interface IQueryHandler
  {
    Task<IEnumerable<SupplierWithRatesDto>> HandleAsync(int? supplierId = null, CancellationToken cancellationToken = default);
  }

  public class QueryHandler(IGetSuppliersWithRatesQuery query) : IQueryHandler
  {
    public async Task<IEnumerable<SupplierWithRatesDto>> HandleAsync(int? supplierId = null, CancellationToken cancellationToken = default)
    {
      SupplierWithRatesProjection[] suppliers = (await query.QueryAsync(cancellationToken)).ToArray();
      SupplierWithRatesProjection[] suppliersToReturn = supplierId.HasValue
        ? suppliers.Where(supplier => supplier.Id == supplierId.Value).ToArray()
        : suppliers;

      if (suppliersToReturn.Length == 0)
      {
        return [];
      }

      RateEntry[] allRates = suppliersToReturn
        .SelectMany(supplier => supplier.SupplierRates.Select(supplierRate => new RateEntry(supplier.Id, supplierRate)))
        .ToArray();

      HashSet<int> overlappingRateIds = supplierId.HasValue
        ? GetOverlappingRateIdsForSupplier(supplierId.Value, allRates)
        : GetOverlappingRateIds(allRates);

      return suppliersToReturn
        .Select(supplier => MapFrom(supplier, overlappingRateIds))
        .Where(supplier => supplier.SupplierRates.Any())
        .ToArray();
    }

    private static HashSet<int> GetOverlappingRateIds(RateEntry[] allRates)
    {
      HashSet<int> overlappingRateIds = [];

      for (int i = 0; i < allRates.Length; i++)
      {
        for (int j = i + 1; j < allRates.Length; j++)
        {
          if (!Overlaps(allRates[i].SupplierRate, allRates[j].SupplierRate))
          {
            continue;
          }

          overlappingRateIds.Add(allRates[i].SupplierRate.Id);
          overlappingRateIds.Add(allRates[j].SupplierRate.Id);
        }
      }

      return overlappingRateIds;
    }

    private static HashSet<int> GetOverlappingRateIdsForSupplier(int supplierId, RateEntry[] allRates)
    {
      HashSet<int> overlappingRateIds = [];
      RateEntry[] supplierRates = allRates
        .Where(rate => rate.SupplierId == supplierId)
        .ToArray();

      foreach (RateEntry supplierRate in supplierRates)
      {
        bool hasOverlap = allRates
          .Where(rate => rate.SupplierRate.Id != supplierRate.SupplierRate.Id)
          .Any(rate => Overlaps(supplierRate.SupplierRate, rate.SupplierRate));

        if (hasOverlap)
        {
          overlappingRateIds.Add(supplierRate.SupplierRate.Id);
        }
      }

      return overlappingRateIds;
    }

    private static SupplierWithRatesDto MapFrom(SupplierWithRatesProjection supplier, HashSet<int> overlappingRateIds)
    {
      return new SupplierWithRatesDto(
        supplier.Id,
        supplier.Name,
        supplier.Address,
        supplier.CreatedByUser,
        supplier.CreatedOn,
        supplier.SupplierRates
          .Where(supplierRate => overlappingRateIds.Contains(supplierRate.Id))
          .Select(supplierRate => new SupplierRateDto(
            supplierRate.Id,
            supplierRate.SupplierId,
            supplierRate.Rate,
            supplierRate.RateStartDate,
            supplierRate.RateEndDate,
            supplierRate.CreatedByUser,
            supplierRate.CreatedOn))
          .ToArray());
    }

    private static bool Overlaps(SupplierRateProjection first, SupplierRateProjection second)
    {
      DateOnly firstEndDate = first.RateEndDate ?? DateOnly.MaxValue;
      DateOnly secondEndDate = second.RateEndDate ?? DateOnly.MaxValue;

      return first.RateStartDate <= secondEndDate && second.RateStartDate <= firstEndDate;
    }

    private sealed record RateEntry(int SupplierId, SupplierRateProjection SupplierRate);
  }
}
