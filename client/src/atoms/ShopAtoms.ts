import { atom } from "jotai";
import { PaperOrderBy, SortOrder, FilterType, PagingInfo, PaperDetailViewModel } from "../Api";

export const ShopSortAtom = atom({
    orderBy: PaperOrderBy.Id,
    sortBy: SortOrder.Desc,
});

export const ShopProductsAtom = atom<PaperDetailViewModel[]>([]);

export const ShopSelectedPropertiesAtom = atom<number[]>([]);

export const ShopFilterTypeAtom = atom<FilterType>(FilterType.And);

// true = discontinued, false = available, null = all
export const ShopDiscontinuedAtom = atom<boolean | null>(false);

export const ShopPagingInfoAtom = atom<PagingInfo>({
    currentPage: 1,
    itemsPerPage: 10,
    totalPages: 1,
    totalItems: 0,
});

export const ShopPriceRangeAtom = atom({
    minPrice: null as number | null,
    maxPrice: null as number | null,
});