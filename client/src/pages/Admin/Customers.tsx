import LeftNavigation from "../../components/Admin/LeftNavigation";
import { api } from "../../http";
import CustomerTableItem from "../../components/Admin/CustomerTableItem";
import { useEffect } from "react";
import { useAtom } from "jotai";
import { AdminCustomerAtom, AdminCustomerPagingInfoAtom } from "../../atoms/AdminUsersAtoms";
import { CustomerPagedViewModel } from "../../Api";
import Pagination from "../../components/Pagination/Pagination";
import PageInfoDisplay from "../../components/Pagination/PageInfoDisplay";
import PageSizeSelector from "../../components/Pagination/PageSizeSelector";


export default function AdminCustomersPage() {
    const [customers, setCustomers] = useAtom(AdminCustomerAtom);
    const [pagingInfo, setPagingInfo] = useAtom(AdminCustomerPagingInfoAtom);

    useEffect(() => {
        api.customer.all({orders: true,
                           page: pagingInfo.currentPage,
                           pageSize: pagingInfo.itemsPerPage
                        })
                    .then(response => {
                        const customerPagedViewModel: CustomerPagedViewModel = {
                            customers: response.data.customers ?? [],
                            pagingInfo: response.data.pagingInfo ?? {
                                currentPage: pagingInfo.currentPage,
                                itemsPerPage: pagingInfo.itemsPerPage,
                                totalPages: pagingInfo.totalPages,
                                totalItems: pagingInfo.totalItems
                            }
                        };
                        setPagingInfo(customerPagedViewModel.pagingInfo)
                        setCustomers(customerPagedViewModel);
                    })
                .catch(error => {
                    console.error('Error fetching customers:', error);
                });
    }, [pagingInfo.currentPage, pagingInfo.itemsPerPage]);

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <LeftNavigation />
            <main className="flex-1 pl-4 pr-4 rounded-box">
            <div className="flex w-auto flex-row items-center">
                <div className="flex mt-2 justify-center">
                    <h3 className="font-bold text-2xl mb-2">Customers</h3>
                </div>
            </div>
            <div className="flex items-center justify-between">
                <PageInfoDisplay currentPage={pagingInfo.currentPage} pageSize={pagingInfo.itemsPerPage} totalItems={pagingInfo.totalItems} />
                <PageSizeSelector
                    pageSize={pagingInfo.itemsPerPage}
                    onPageSizeChange={(size) => {
                        setPagingInfo(prev => ({
                            ...prev,
                            itemsPerPage: size,
                            currentPage: 1,
                        }));
                    }}
                />
            </div>
            <div className="grid grid-cols-1 gap-3">
            <div className="overflow-x-auto rounded-box border border-base-300 bg-base-100 p-0">
                <table className="table w-full">
                <thead>
                <tr>
                    <th>Customer</th>
                    <th>Contact</th>
                    <th>Total orders</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {customers?.customers.map((customer, index) => (
                    <CustomerTableItem key={index} customer={customer}/>
                ))}
                </tbody>
            </table>
            </div>

                {pagingInfo.totalPages > 1 && (
                    <Pagination
                        currentPage={pagingInfo.currentPage}
                        totalPages={pagingInfo.totalPages}
                        onPageChange={(page) => setPagingInfo(prev => ({ ...prev, currentPage: page }))}
                    />
                )}
            </div>
            </main>
        </div>
        </div>
    )
}