import { useEffect, useState } from "react";
import { api } from "../http";
import { PaperDetailViewModel, FilterType, PaperOrderBy, SortOrder } from "../Api";
import { useAtom } from "jotai";
import { IsBackendReachableAtom } from "../atoms/IsBackendReachableAtom";

interface UseFetchPapersParams {
    page?: number;
    pageSize?: number;
    search?: string | null;
    discontinued?: boolean | null;
    orderBy?: PaperOrderBy;
    sortBy?: SortOrder;
    filter?: string | null;
    filterType?: FilterType;
    minPrice?: number | null;
    maxPrice?: number | null;
    refresh?: boolean;
}

export function useFetchPapers({page = 1, pageSize = 10, search = null, discontinued = null, 
                                orderBy = PaperOrderBy.Id, sortBy = SortOrder.Asc, filter = null, filterType = FilterType.Or,
                                minPrice = null, maxPrice = null, refresh = false}: UseFetchPapersParams) {
    const [isBackendReachable] = useAtom(IsBackendReachableAtom);
    const [papers, setPapers] = useState<PaperDetailViewModel[]>([]);
    const [loading, setLoading] = useState(false);
    const [totalPages, setTotalPages] = useState(1);
    const [totalItems, setTotalItems] = useState(0);

    useEffect(() => {
        const fetchPapers = async () => {
            if (!isBackendReachable) return;

            setLoading(true);

            api.paper.all({page, pageSize, search, discontinued, orderBy, sortBy, filter, filterType, minPrice, maxPrice}).then((response) => {
                    setPapers(response.data.papers ?? []);
                    setTotalPages(response.data.pagingInfo?.totalPages ?? 1);
                    setTotalItems(response.data.pagingInfo?.totalItems ?? 0);

                    if (page > (response.data.pagingInfo?.totalPages ?? 1)) {
                        setPapers([]);
                    }
                }).catch((error) => {
                    console.error("Error fetching papers:", error);
                }).finally(() => {
                    setLoading(false);
                });
        };

        fetchPapers();
    }, [isBackendReachable, page, pageSize, search, discontinued, orderBy, sortBy, filter, filterType, minPrice, maxPrice, refresh]);

    return { papers, loading, totalPages, totalItems };
}