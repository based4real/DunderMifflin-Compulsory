import { FaBox, FaShoppingCart } from "react-icons/fa";

export default function ShopCartProduct() {
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
            <FaBox className="mr-2 text-primary" />
            <span className="text-gray-500">500 in stock</span>
          </p>
          <div className="flex items-center">
            <button className="btn btn-square">
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-6 w-6"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    d="M6 18L18 6M6 6l12 12" />
                </svg>
                </button>
                <div className="divider divider-horizontal"></div>
                <label className="input input-bordered flex items-center gap-2">
                    <input type="number" min={1} step={1} className="grow w-12" placeholder="1" />
                    <span className="badge badge-primary">Items</span>
                </label>
            </div>
        </div>

        <div className="flex flex-col items-end space-y-3">
          <p className="text-xl font-bold w-full text-center">$430</p>
        </div>
      </div>
    </div>
  );
}
