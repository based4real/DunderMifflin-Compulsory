import { useState } from 'react';
import { FaPlus, FaTag, FaTimes, FaBan, FaWarehouse } from "react-icons/fa";
import LeftNavigation from "../../components/Admin/LeftNavigation";
import ProductTableItem from "../../components/Admin/ProductTableItem";
import CreateProductModal from "../../components/Admin/CreateProductModal";
import RestockProductModal from "../../components/Admin/RestockProductModal";
import CreatePropertyModal from "../../components/Admin/CreatePropertyModal";
import Pagination from "../../components/Pagination/Pagination";
import PageSizeSelector from "../../components/Pagination/PageSizeSelector";
import ClearableSearch from "../../components/Input/ClearableSearch";
import { useFetchPapers } from "../../hooks/useFetchPapers";
import { useProductSelection } from '../../hooks/useProductSelection';
import { api } from "../../http";
import { toast } from "react-hot-toast";
import { useAtom } from 'jotai';
import { CartAtom } from '../../atoms/CartAtom';
import { PaperOrderBy, SortOrder } from "../../Api";

export default function AdminProductsPage() {
    const [isCreateProductModalOpen, setIsCreateProductModalOpen] = useState(false);
    const [isRestockModalOpen, setIsRestockModalOpen] = useState(false);
    const [isCreatePropertyModalOpen, setIsCreatePropertyModalOpen] = useState(false);
    const [restockProductId, setRestockProductId] = useState<number[] | null>(null);
    const [restockProductName, setRestockProductName] = useState<string>("");
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [searchTerm, setSearchTerm] = useState("");
    const [refresh, setRefresh] = useState(false);
    const [cart, setCart] = useAtom(CartAtom);
    
    const { papers, loading, totalPages, totalItems } = useFetchPapers({
        page: currentPage,
        pageSize,
        search: searchTerm,
        orderBy: PaperOrderBy.Id,
        sortBy: SortOrder.Desc,
        refresh
    });

    const {
        selectedProductIds,
        selectAll,
        handleSelectAll,
        handleToggleSelect,
        handleDeselectAll,
        resetSelection
    } = useProductSelection(papers);

    const refreshProducts = () => {
        setRefresh(prev => !prev);
        resetSelection();
    };

    const closeCreateProductModal = () => { 
        setIsCreateProductModalOpen(false);
        refreshProducts();
    };
    
    const closeRestockModal = () => {
        setIsRestockModalOpen(false);
        refreshProducts();
    };

    const closeCreatePropertyModal = () => {
        setIsCreatePropertyModalOpen(false);
        refreshProducts();
    };

    const openRestockModal = (productId: number, productName: string) => {
        setRestockProductId([productId]);
        setRestockProductName(productName);
        setIsRestockModalOpen(true);
    };
    
    const handleBulkRestock = () => {
        if (selectedProductIds.length === 0) {
            toast.error("No products selected for bulk restock.");
            return;
        }
        const selectedProductNames = getSelectedProductNames();
        setRestockProductId(selectedProductIds);
        setRestockProductName(selectedProductNames);
        setIsRestockModalOpen(true);
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
            })
            .catch(() => {
                toast.error("Failed to discontinue selected products.");
            });
    };

    const removeProductsFromCart = (productIds: number[]) => {
        const updatedCartEntries = cart.cartEntries.filter(
            (entry) => !productIds.includes(entry.paper.id)
        );
        const updatedCart = { ...cart, cartEntries: updatedCartEntries };
        setCart(updatedCart);
        localStorage.setItem('cartItems', JSON.stringify(updatedCart));
    };

    const getSelectedProductNames = () => {
        return papers.filter(paper => selectedProductIds.includes(paper.id))
                     .map(paper => paper.name)
                     .join(", ");
    };

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
            <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            
                <LeftNavigation />
                
                <main className="flex-1 pl-4 pr-4 rounded-box">
                    <div className="flex w-auto flex-row items-center justify-between">
                        <div className="flex mt-2 justify-center">
                            <h3 className="font-bold text-2xl mb-2">Products</h3>
                        </div>
                    </div>
                    
                    <div className="flex items-center justify-between mb-4">
                        <div className="w-64">
                            <ClearableSearch
                                searchTerm={searchTerm}
                                onSearchChange={setSearchTerm}
                                onClearSearch={() => setSearchTerm('')}
                                promptText="Search products..."
                            />
                        </div>
                        <PageSizeSelector
                            pageSize={pageSize}
                            onPageSizeChange={(size) => {
                                setPageSize(size);
                                setCurrentPage(1);
                            }}
                        />
                    </div>

                    <div className="mb-4 flex justify-between items-center">
                        <button className="btn btn-sm btn-secondary"
                                onClick={handleDeselectAll}
                                disabled={selectedProductIds.length === 0}>
                            <FaTimes className="mr-1" />
                            Deselect All
                        </button>

                        <div className="flex space-x-2">
                            <button className="btn btn-sm btn-accent"
                                    onClick={handleBulkRestock}
                                    disabled={selectedProductIds.length === 0}>
                                <FaWarehouse className="mr-1" />
                                Restock Selected
                            </button>
                            <button className="btn btn-sm btn-error"
                                    onClick={handleBulkDiscontinue}
                                    disabled={selectedProductIds.length === 0}>
                                <FaBan className="mr-1" />
                                Discontinue Selected
                            </button>
                            <button className="btn btn-sm btn-primary" onClick={() => setIsCreatePropertyModalOpen(true)}>
                                <FaTag />
                            </button>
                            <button className="btn btn-sm btn-primary" onClick={() => setIsCreateProductModalOpen(true)}>
                                <FaPlus />
                            </button>
                        </div>
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
                                            onRestock={openRestockModal}
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
                isOpen={isCreateProductModalOpen}
                onClose={closeCreateProductModal}
            />

            <RestockProductModal
                isOpen={isRestockModalOpen}
                onClose={closeRestockModal}
                productIds={restockProductId || []}
                productNames={restockProductName}
            />

            <CreatePropertyModal
                isOpen={isCreatePropertyModalOpen}
                onClose={closeCreatePropertyModal}
            />
        </div>
    );
}
