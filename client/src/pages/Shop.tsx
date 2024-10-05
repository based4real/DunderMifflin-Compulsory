import ShopProduct from "../components/Shop/ShopProduct";
import ShopSortDropDown from "../components/Shop/ShopSortDropdown";

export default function ShopPage() {

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <aside className="w-64 bg-base-100 border border-base-300 p-4 shadow-md rounded-box lg:-ml-32">
            <h2 className="text-lg font-bold mb-4">Filters</h2>
            <div className="form-control mb-4">
                <label className="label cursor-pointer">
                <input type="checkbox" className="checkbox checkbox-primary" />
                <span className="label-text ml-2">Category 1</span>
                </label>

                <label className="label cursor-pointer">
                <input type="checkbox" className="checkbox checkbox-primary" />
                <span className="label-text ml-2">Category 2</span>
                </label>

                <label className="label cursor-pointer">
                <input type="checkbox" className="checkbox checkbox-primary" />
                <span className="label-text ml-2">Category 3</span>
                </label>
            </div>

            <div className="form-control mb-4">
                <h3 className="font-semibold mb-2">Price Range</h3>
                <label className="label cursor-pointer">
                <input type="checkbox" className="checkbox checkbox-primary" />
                <span className="label-text ml-2">$0 - $50</span>
                </label>

                <label className="label cursor-pointer">
                <input type="checkbox" className="checkbox checkbox-primary" />
                <span className="label-text ml-2">$50 - $100</span>
                </label>
            </div>
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
            <div className="grid grid-cols-1 gap-3">
            {Array.from({ length: 5 }, (_, index) => (
                <ShopProduct key={index}/>
            ))}
            <div className="flex justify-center align-middle">
                <div className="flex join">
                <button className="join-item btn bg-base-100">1</button>
                <button className="join-item btn btn-active">2</button>
                <button className="join-item btn bg-base-100">3</button>
                <button className="join-item btn bg-base-100">4</button>
                </div>
            </div>
            </div>
            </main>
        </div>
        </div>
    )
}