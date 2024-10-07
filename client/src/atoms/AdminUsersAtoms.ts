import { atom } from "jotai";
import { CustomerDetailViewModel, PagingInfo } from "../Api";

export const AdminCustomerAtom = atom<CustomerDetailViewModel[]>([]);

export const AdminCustomerPagingInfoAtom = atom<PagingInfo>({
    currentPage: 1,
    itemsPerPage: 10,
    totalPages: 1,
    totalItems: 0,
});