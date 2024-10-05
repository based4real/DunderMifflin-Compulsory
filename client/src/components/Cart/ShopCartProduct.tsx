import { FaBox, FaShoppingCart } from "react-icons/fa";

export default function ShopCartProduct() {
  return (
    <div className="bg-base-100 shadow-lg rounded-box p-6 flex flex-col justify-between h-full">
      <div className="flex justify-between items-start">
        <div className="space-y-4 flex-grow">
          <div className="space-y-2">
            <h3 className="font-bold text-lg">Paper</h3>
            <p className="text-sm text-gray-500">
              Water Resistant, A4, Bulletproof
            </p>
          </div>

          <p className="text-sm flex items-center">
            <FaBox className="mr-2 text-primary" />
            <span className="text-gray-500">500 in stock</span>
          </p>
        </div>

        <div className="flex flex-col items-end space-y-3">
          <p className="text-xl font-bold text-primary w-full text-center">$430</p>
          <button className="btn btn-primary w-full flex items-center justify-center gap-2">
            <FaShoppingCart />
            Add to cart
          </button>
        </div>
      </div>
    </div>
  );
}
