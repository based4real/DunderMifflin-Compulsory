import { FaBox, FaShoppingCart, FaWarehouse, FaBan, FaTimesCircle, FaBell } from "react-icons/fa";
import QuantityInput from "../Input/QuantityInput";
import { PaperDetailViewModel } from "../../Api";
import { useState } from "react";
import { toast } from "react-hot-toast";

export default function ShopProduct({ paper, addToCart }: { paper: PaperDetailViewModel, addToCart: (paper: PaperDetailViewModel, quantity: number) => void; }) {
  const [quantity, setQuantity] = useState<number>(1);

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

          {paper.discontinued ? (
              <p className="text-sm flex items-center text-error">
                <FaBan className="mr-2" />
                <span>Discontinued</span>
              </p>
          ) : paper.stock === 0 ? (
              <p className="text-sm flex items-center text-warning">
                <FaTimesCircle className="mr-2" />
                <span>Out of stock</span>
              </p>
          ) : (
              <p className="text-sm flex items-center">
                <FaWarehouse className="mr-2 text-primary" />
                <span className="text-gray-500">{paper.stock} in stock</span>
              </p>
          )}
        </div>

        <div className="flex flex-col items-end space-y-3">
          <p className="text-xl font-bold text-primary w-auto text-center">${paper.price.toFixed(2)}</p>
          <div className="flex gap-4 flex-row">
          
          {!paper.discontinued && paper.stock > 0 && (
              <label className="input input-bordered w-auto flex items-center gap-2">
                <FaBox />
                <QuantityInput max={paper.stock} value={quantity} onChange={setQuantity} />
              </label>
          )}

            <button
                disabled={paper.discontinued}
                className="btn btn-primary flex items-center justify-center gap-2"
                onClick={() => {
                  if (paper.stock === 0) {
                    toast(`You will be notified when ${paper.name} is back in stock.`);
                  } else {
                    addToCart(paper, quantity);
                  }
                }}
            >
              {paper.stock === 0 ? (
                  <>
                    <FaBell />
                    Notify me
                  </>
              ) : (
                  <>
                    <FaShoppingCart />
                    Add to cart
                  </>
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}