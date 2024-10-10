import { useState } from 'react';
import { FaPlus } from "react-icons/fa";
import LeftNavigation from "../../components/Admin/LeftNavigation";
import ProductTableItem from "../../components/Admin/ProductTableItem";
import CreateProductModal from "../../components/Admin/CreateProductModal";
import { useFetchPapers } from "../../hooks/useFetchPapers";
import Pagination from "../../components/Pagination/Pagination";
import { PaperOrderBy, SortOrder } from "../../Api";
import ClearableSearch from "../../components/Input/ClearableSearch";
import { api } from "../../http";
import { toast } from "react-hot-toast";
import { useAtom } from 'jotai';
import { CartAtom } from '../../atoms/CartAtom'

export default function AdminProductsPage() {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(10);
    const [searchTerm, setSearchTerm] = useState("");
    const [refresh, setRefresh] = useState(false);
    const [selectedProductIds, setSelectedProductIds] = useState<number[]>([]);
    const [selectAll, setSelectAll] = useState(false);
    const [cart, setCart] = useAtom(CartAtom);
    
    const { papers, loading, totalPages } = useFetchPapers({
        page: currentPage,
        pageSize,
        search: searchTerm,
        orderBy: PaperOrderBy.Id,
        sortBy: SortOrder.Desc,
        refresh
    });

    const refreshProducts = () => {
        setRefresh(prev => !prev);
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
        refreshProducts();
    };

    const handleSelectAll = (e: React.ChangeEvent<HTMLInputElement>) => {
        const isChecked = e.target.checked;
        setSelectAll(isChecked);
        setSelectedProductIds(isChecked ? papers.map(paper => paper.id) : []);
    };

    const handleDeselectAll = () => {
        setSelectedProductIds([]);
        setSelectAll(false);
    };

    const handleToggleSelect = (isSelected: boolean, productId: number) => {
        setSelectedProductIds((prevSelectedIds) => {
            const updatedSelectedIds = isSelected
                ? [...prevSelectedIds, productId]
                : prevSelectedIds.filter((id) => id !== productId);
            
            setSelectAll(updatedSelectedIds.length === papers.length);

            return updatedSelectedIds;
        });
    };

    const handleDiscontinue = (paperId: number) => {
        api.paper.discontinue(paperId)
            .then(() => {
                toast.success("Product has been discontinued.");
                refreshProducts();
                removeProductsFromCart([paperId]);
            })
            .catch(() => {
                toast.error("Failed to discontinue the product.");
            });
    };

    const handleBulkDiscontinue = () => {
        if (selectedProductIds.length === 0) {
            toast.error("No products selected for bulk discontinue.");
            return;
        }

        api.paper.discontinueBulk(selectedProductIds)
            .then(() => {
                toast.success("Selected products have been discontinued.");
                refreshProducts();
                removeProductsFromCart(selectedProductIds);
                setSelectedProductIds([]);
                setSelectAll(false);
            })
            .catch(() => {
                toast.error("Failed to discontinue selected products.");
            });
    };

    const removeProductsFromCart = (productIds: number[]) => {
        const updatedCartEntries = cart.cartEntries.filter(
            (entry) => !productIds.includes(entry.paper.id)
        );

        const updatedCart = {
            ...cart,
            cartEntries: updatedCartEntries,
        };

        setCart(updatedCart);
        localStorage.setItem('cartItems', JSON.stringify(updatedCart));
    };

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
            <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            
                <LeftNavigation />
                
                <main className="flex-1 pl-4 pr-4 rounded-box">
                    <div className="flex w-auto flex-row items-center">
                        <div className="flex mt-2 justify-center">
                            <h3 className="font-bold text-2xl mb-2">Products</h3>
                        </div>
                    </div>

                    <div className="flex justify-between items-center mb-4">
                        <div className="w-64">
                            <ClearableSearch
                                searchTerm={searchTerm}
                                onSearchChange={setSearchTerm}
                                onClearSearch={() => setSearchTerm('')}
                                promptText="Search products..."
                            />
                        </div>
                        <button className="btn btn-sm btn-primary" onClick={() => setIsModalOpen(true)}>
                            <FaPlus />
                        </button>
                    </div>

                    <div className="mb-4 flex space-x-2">
                        <button
                            className="btn btn-sm btn-danger"
                            onClick={handleBulkDiscontinue}
                            disabled={selectedProductIds.length === 0}>
                            Discontinue Selected
                        </button>
                        <button
                            className="btn btn-sm btn-secondary"
                            onClick={handleDeselectAll}
                            disabled={selectedProductIds.length === 0}>
                            Deselect All
                        </button>
                    </div>

                    {loading ? (
                        <div className="flex justify-center">Loading...</div>
                    ) : (
                        <div className="overflow-x-auto rounded-box border border-base-300 bg-base-100 p-0">
                            <table className="table w-full">
                                <thead>
                                <tr>
                                    <th>
                                        <label>
                                            <input
                                                type="checkbox"
                                                className="checkbox"
                                                checked={selectAll}
                                                onChange={handleSelectAll}
                                            />
                                        </label>
                                    </th>
                                    <th>Name</th>
                                    <th>Properties</th>
                                    <th>Stock</th>
                                    <th>Price</th>
                                    <th></th>
                                </tr>
                                </thead>
                                <tbody>
                                {papers.length === 0 ? (
                                    <tr>
                                        <td colSpan={6} className="text-center">
                                            No products found.
                                        </td>
                                    </tr>
                                ) : (
                                    papers.map((paper) => (
                                        <ProductTableItem
                                            key={paper.id}
                                            paper={paper}
                                            selected={selectedProductIds.includes(paper.id)}
                                            onDiscontinue={handleDiscontinue}
                                            onToggleSelect={(isSelected) => handleToggleSelect(isSelected, paper.id)}
                                        />
                                    ))
                                )}
                                </tbody>
                            </table>
                        </div>
                    )}

                    {!loading && papers.length > 0 && totalPages > 1 && (
                        <Pagination
                            currentPage={currentPage}
                            totalPages={totalPages}
                            onPageChange={(page) => setCurrentPage(page)}
                        />
                    )}
                </main>
            </div>
            
            <CreateProductModal
                isOpen={isModalOpen}
                onClose={handleModalClose}
            />
        </div>
    );
}
