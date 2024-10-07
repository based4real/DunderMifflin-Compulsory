import { FaWarehouse } from "react-icons/fa";
import { CartOrderEntry } from "../../model/CartModel";

export default function ShopCartProduct({order, removeFromCart, updateQuantity}: {order: CartOrderEntry, removeFromCart: (order: CartOrderEntry) => void, updateQuantity: (order: CartOrderEntry, newQuantity: number) => void;}) {

  const paper = order.paper;
  const total = paper.price * order.quantity;

  const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newQuantity = Math.max(1, Number(event.target.value));

    if (paper.stock < newQuantity)
      return;
    
    updateQuantity(order, newQuantity);
  };

  return (
    <div className="border-base-300 bg-base-100 border shadow-lg rounded-box p-6 flex flex-col justify-between h-full">
      <div className="flex justify-between items-start">
        <div className="space-y-4 flex-grow">
          <div className="space-y-2">
            <h3 className="font-bold text-lg">{paper.name}</h3>
            <p className="text-sm text-gray-500">
              {paper.properties && paper.properties.length > 0
                ? `Hello, ${paper.properties.map((prop) => prop.name).join(", ")}`
                : "No properties"}
            </p>
          </div>

          <p className="text-sm flex items-center">
            <FaWarehouse className="mr-2 text-primary" />
            <span className="text-gray-500">{paper.stock} in stock</span>
          </p>
          <div className="flex items-center">
            <button className="btn btn-square" onClick={() => removeFromCart(order)}>
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
                    <input type="number" min={1} max={paper.stock} step={1} className="grow w-12" onChange={handleQuantityChange} value={order.quantity} />
                    <span className="badge badge-neutral">Item(s)</span>
                </label>
            </div>
        </div>

        <div className="flex flex-col justify-between items-end h-full">
          <p className="text-xl font-bold text-center">${paper.price.toFixed(2)}</p>
          
          <p className="text-xl font-bold text-center">Total: ${total.toFixed(2)}</p>
      </div>
      </div>
    </div>
  );
}
