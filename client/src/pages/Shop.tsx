import ShopProduct from "../components/Shop/ShopProduct";

export default function ShopPage() {
    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <aside className="w-64 bg-base-100 p-4 shadow-md lg:-ml-32">
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

            <main className="flex-1 p-4">
            <div className="grid grid-cols-1 gap-6">
            {Array.from({ length: 10 }, (_, index) => (
                    <ShopProduct key={index}/>
                ))}
            </div>
            </main>
        </div>
        </div>

    )
}