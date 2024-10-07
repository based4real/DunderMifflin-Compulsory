import { FaBox, FaShoppingCart, FaWarehouse } from "react-icons/fa";
import QuantityInput from "../Input/QuantityInput";
import { PaperDetailViewModel } from "../../Api";

export default function ShopProduct({ paper }: { paper: PaperDetailViewModel }) {
  return (
    <div className="border-base-300 bg-base-100 border shadow-lg rounded-box p-6 flex flex-col justify-between h-full">
      <div className="flex justify-between items-start">
        <div className="space-y-4 flex-grow">
          <div className="space-y-2">
            <h3 className="font-bold text-lg">{paper.name}</h3>
            <p className="text-sm text-gray-500">
              {paper.properties && paper.properties.length > 0
                  ? paper.properties.map((prop) => prop.name).join(", ")
                  : "No properties"}
            </p>
          </div>

          <p className="text-sm flex items-center">
            <FaWarehouse className="mr-2 text-primary" />
            <span className="text-gray-500">{paper.stock} in stock</span>
          </p>
        </div>

        <div className="flex flex-col items-end space-y-3">
          <p className="text-xl font-bold text-primary w-auto text-center">${paper.price.toFixed(2)}</p>
          <div className="flex gap-4 flex-row">
          <label className="input input-bordered w-auto flex items-center gap-2">
          <FaBox />
          <QuantityInput max={paper.stock} />
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