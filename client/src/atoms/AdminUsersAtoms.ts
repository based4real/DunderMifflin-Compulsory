import { atom } from "jotai";
import { CustomerPagedViewModel, PagingInfo } from "../Api";

export const AdminCustomerAtom = atom<CustomerPagedViewModel>();

export const AdminCustomerPagingInfoAtom = atom<PagingInfo>({
    currentPage: 1,
    itemsPerPage: 10,
    totalPages: 1,
    totalItems: 0,
});