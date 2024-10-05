import { FaPlus } from "react-icons/fa";
import LeftNavigation from "../../components/Admin/LeftNavigation";
import ProductTableItem from "../../components/Admin/ProductTableItem";

export default function AdminProductsPage() {
    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <LeftNavigation />
            <main className="flex-1 pl-4 pr-4 rounded-box">
            <div className="flex w-auto flex-row items-center">
                <div className="flex mt-2 justify-center">
                    <h3 className="font-bold text-2xl mb-2">Products</h3>
                </div>
                <div className="flex w-full justify-end">
                    <button className="btn btn-sm btn-primary">
                        <FaPlus />
                    </button>
                </div>
            </div>
            <div className="grid grid-cols-1 gap-3">
            <div className="overflow-x-auto">
            <table className="table bg-base-100 border border-base-300">
                <thead>
                <tr>
                    <th>
                    <label>
                        <input type="checkbox" className="checkbox" />
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
                {Array.from({ length: 5 }, (_, index) => (
                    <ProductTableItem key={index}/>
                ))}
                </tbody>
            </table>
            </div>
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