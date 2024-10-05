import { FaBox, FaShoppingCart, FaWarehouse } from "react-icons/fa"; 
import { FaBoxArchive } from "react-icons/fa6";

export default function ShopProduct() {
  return (
    <div className="border-base-300 bg-base-100 border shadow-lg rounded-box p-6 flex flex-col justify-between h-full">
      <div className="flex justify-between items-start">
        <div className="space-y-4 flex-grow">
          <div className="space-y-2">
            <h3 className="font-bold text-lg">Paper</h3>
            <p className="text-sm text-gray-500">
              Water Resistant, A4, Bulletproof
            </p>
          </div>

          <p className="text-sm flex items-center">
            <FaWarehouse className="mr-2 text-primary" />
            <span className="text-gray-500">500 in stock</span>
          </p>
        </div>

        <div className="flex flex-col items-end space-y-3">
          <p className="text-xl font-bold text-primary w-auto text-center">$430</p>
          <div className="flex gap-4 flex-row">
          <label className="input input-bordered w-auto flex items-center gap-2">
          <FaBox />

          <input type="number" className="grow w-12" step={1} min={1} placeholder="1" />
          </label>
          <button className="btn btn-primary flex items-center justify-center gap-2">
            <FaShoppingCart />
            Add to cart
          </button>
          </div>
        </div>
      </div>
    </div>
  );
}
