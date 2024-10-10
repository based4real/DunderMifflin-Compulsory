import { api } from "../../http";
import { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { CustomerDetailViewModel, CustomerOrderPagedViewModel, CustomerPagedViewModel } from "../../Api";
import { useParams } from "react-router-dom";
import LeftNavigation from "../../components/Admin/LeftNavigation";
import { FaSearch } from "react-icons/fa";
import CustomerOrderTableItem from "../../components/Admin/CustomerOrderTableItem";
import Pagination from "../../components/Pagination/Pagination";
import PageInfoDisplay from "../../components/Pagination/PageInfoDisplay";
import PageSizeSelector from "../../components/Pagination/PageSizeSelector";


export default function AdminCustomerDetailsPage() {
    const [customer, setCustomer] = useState<CustomerOrderPagedViewModel | null>(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);

    const { id } = useParams();

    useEffect(() => {
        if (id) {
            const stringId = Number(id);
            api.customer.getCustomerWithOrders(stringId, {
                page: currentPage,
                pageSize: itemsPerPage,
            })
            .then(response => {
                setCustomer(response.data);
                setTotalPages(response.data.pagingInfo.totalPages ?? 1);
            })
            .catch(error => {
                console.error("Error fetching customer details: ", error);
            });
        }
    }, [id, currentPage, itemsPerPage]);

    const handlePageChange = (page: number) => {
        if (page >= 1 && page <= totalPages)
            setCurrentPage(page);
    };

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <LeftNavigation />
            <main className="flex-1 pl-4 pr-4 rounded-box">
            <div className="flex w-auto flex-row items-center">
                <div className="flex mt-2 justify-center">
                    <h3 className="font-bold text-2xl mb-2">Order history: {customer?.customerDetails.name}</h3>
                </div>
            </div>
            <div className="flex items-center justify-between">
                <PageInfoDisplay currentPage={currentPage} pageSize={itemsPerPage} totalItems={totalPages} />
                <PageSizeSelector
                    pageSize={itemsPerPage}
                    onPageSizeChange={(size) => {
                        setCurrentPage(1);
                        setItemsPerPage(size);
                    }}
                />
            </div>
            <div className="grid grid-cols-1 gap-3">
            <div className="overflow-x-auto rounded-box border border-base-300 bg-base-100 p-0">
                <table className="table w-full">
                <thead>
                <tr>
                    <th>ID</th>
                    <th>Order Date</th>
                    <th>Expected Delivery</th>
                    <th>Items</th>
                    <th>Total Price</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {customer?.customerDetails?.orders?.length ? (
                    customer.customerDetails.orders.map((order, index) => (
                        <CustomerOrderTableItem key={index} order={order}/>
                    ))
                    ) : (
                        <strong>No orders available</strong>
                    )}
                </tbody>
            </table>
            </div>
            <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={handlePageChange}
                />
            </div>
            </main>
        </div>
        </div>
    )
}