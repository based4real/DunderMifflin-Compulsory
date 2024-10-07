import { useCallback, useEffect, useState } from "react";
import { useAtom } from "jotai";
import { api } from "../http";
import { ShopSortAtom, ShopProductsAtom, ShopSelectedPropertiesAtom, ShopFilterTypeAtom, ShopDiscontinuedAtom, ShopPagingInfoAtom, ShopPriceRangeAtom, ShopSearchAtom } from "../atoms/ShopAtoms";
import { IsBackendReachableAtom } from "../atoms/IsBackendReachableAtom";
import ShopProduct from "../components/Shop/ShopProduct";
import ShopSkeletonProduct from "../components/Shop/ShopSkeletonProduct";
import ShopSortDropDown from "../components/Shop/ShopSortDropdown";
import ShopFilterPanel from "../components/Shop/ShopFilterPanel";
import Pagination from "../components/Pagination/Pagination";
import PageSizeSelector from "../components/Pagination/PageSizeSelector"
import PageInfoDisplay from "../components/Pagination/PageInfoDisplay";
import { CartAtom } from "../atoms/CartAtom";
import { PaperDetailViewModel } from "../Api";
import { toast } from "react-hot-toast";

export default function ShopPage() {
    const [isBackendReachable] = useAtom(IsBackendReachableAtom);
    const [loading, setLoading] = useState(false);
    const [sort] = useAtom(ShopSortAtom);
    const [papers, setPapers] = useAtom(ShopProductsAtom);
    const [selectedProperties, setSelectedProperties] = useAtom(ShopSelectedPropertiesAtom);
    const [filterType, setFilterType] = useAtom(ShopFilterTypeAtom);
    const [discontinued, setDiscontinued] = useAtom(ShopDiscontinuedAtom);
    const [pagingInfo, setPagingInfo] = useAtom(ShopPagingInfoAtom);
    const [priceRange, setPriceRange] = useAtom(ShopPriceRangeAtom);
    const [searchTerm] = useAtom(ShopSearchAtom);

    const [cart, setCart] = useAtom(CartAtom);

    const addToCart = useCallback((paper: PaperDetailViewModel, quantity: number) => {
        const updatedCart = { ...cart };
        if (!updatedCart.cartEntries)
            updatedCart.cartEntries = [];

        if (paper.stock === 0)
            return;

        if (paper.stock < quantity)
            return;

        if (paper.discontinued)
            return;
        
        const existingEntry = updatedCart.cartEntries.find(entry => entry.paper.id === paper.id);
        
        if (existingEntry)
            existingEntry.quantity += quantity;
        else
            updatedCart.cartEntries.push({ paper, quantity });
        
        setCart(updatedCart);
        localStorage.setItem('cartItems', JSON.stringify(updatedCart));
        toast.success(`${quantity} ${paper.name} added to cart!`);
    }, [cart, setCart]);

    useEffect(() => {
        const fetchPapers = async () => {
            if (!isBackendReachable) return;

            setLoading(true);
            const { orderBy, sortBy } = sort;

            api.paper
                .all({
                    page: pagingInfo.currentPage,
                    pageSize: pagingInfo.itemsPerPage,
                    filter: selectedProperties.join(","),
                    filterType,
                    orderBy,
                    sortBy,
                    discontinued,
                    minPrice: priceRange.minPrice,
                    maxPrice: priceRange.maxPrice,
                    search: searchTerm,
                })
                .then((response) => {
                    setPapers(response.data.papers ?? []);
                    setPagingInfo((prev) => ({
                        ...prev,
                        totalPages: response.data.pagingInfo?.totalPages ?? 1,
                        totalItems: response.data.pagingInfo?.totalItems ?? 0,
                    }));

                    if (pagingInfo.currentPage > (response.data.pagingInfo?.totalPages ?? 1)) {
                        setPagingInfo((prev) => ({
                            ...prev,
                            currentPage: 1,
                        }));
                    }
                })
                .catch((error) => {
                    console.error("Error fetching papers: ", error);
                })
                .finally(() => {
                    setLoading(false);
                });
        };

        fetchPapers();
    }, [isBackendReachable, pagingInfo.currentPage, pagingInfo.itemsPerPage, selectedProperties, filterType, sort, discontinued, priceRange, searchTerm]);
    
    useEffect(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }, [pagingInfo.currentPage]);

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
            <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
                <aside className="w-64 bg-base-100 border border-base-300 p-4 shadow-md rounded-box lg:-ml-32">
                    <ShopFilterPanel
                        discontinued={discontinued}
                        onDiscontinuedChange={setDiscontinued}
                        filterType={filterType}
                        onFilterTypeChange={setFilterType}
                        selectedProperties={selectedProperties}
                        onPropertiesChange={setSelectedProperties}
                        minPrice={priceRange.minPrice}
                        maxPrice={priceRange.maxPrice}
                        onPriceRangeChange={(min, max) => setPriceRange({ minPrice: min, maxPrice: max })}
                    />
                </aside>

                <main className="flex-1 pl-4 pr-4 rounded-box">
                    <div className="flex w-auto flex-row items-center">
                        <div className="flex mt-2 align-middle justify-center">
                            <h3 className="font-bold text-2xl mb-2">Products</h3>
                        </div>
                        <div className="flex w-full justify-end mb-2 mt-2">
                            <ShopSortDropDown />
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
                    
                    {loading ? (
                        <div className="grid grid-cols-1 gap-3">
                            {[...Array(4)].map((_, index) => (
                                <ShopSkeletonProduct key={index} />
                            ))}
                        </div>
                    ) : papers.length === 0 ? (
                        <div className="flex justify-center mt-4">
                            <span className="text-lg font-semibold">No products found.</span>
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 gap-3">
                            {papers.map((paper) => (
                                <ShopProduct key={paper.id} paper={paper} addToCart={addToCart} />
                            ))}
                        </div>
                    )}

                    {!loading && papers.length > 0 && pagingInfo.totalPages > 1 && (
                        <Pagination
                            currentPage={pagingInfo.currentPage}
                            totalPages={pagingInfo.totalPages}
                            onPageChange={(page) => setPagingInfo(prev => ({ ...prev, currentPage: page }))}
                        />
                    )}
                </main>
            </div>
        </div>
    )
}