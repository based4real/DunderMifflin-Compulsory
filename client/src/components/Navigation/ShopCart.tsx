
import { useAtom } from "jotai";
import { useNavigate } from "react-router-dom"
import { CartAtom } from "../../atoms/CartAtom";

export default function ShopCart () {
    const [ cartAtom, setCartAtom ] = useAtom(CartAtom);
    const navigate = useNavigate();

    const totalQuantity = cartAtom.cartEntries?.reduce(
      (acc, entry) => acc + entry.quantity,
      0
    ) || 0;

    const totalAmount = cartAtom.cartEntries.reduce((total, entry) => {
      return total + (entry.paper.price * entry.quantity);
  }, 0) || 0;
    const totalFixed = totalAmount.toFixed(2);


    return (
        <div className="dropdown dropdown-end">
        <div tabIndex={0} role="button" className="btn btn-ghost btn-circle">
          <div className="indicator">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-5 w-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            {totalQuantity > 0 &&
                <span className="badge badge-sm indicator-item">{totalQuantity}</span>
            }
        </div>
        </div>
        <div
          tabIndex={0}
          className="card card-compact dropdown-content bg-base-100 z-[1] mt-3 w-52 shadow">
          <div className="card-body">
        {totalQuantity > 0 ? (
            <>
            <span className="text-lg font-bold">{totalQuantity} Items</span>
            <span className="text-info">Subtotal: ${totalFixed}</span>
            <div className="card-actions">
              <button className="btn btn-primary btn-block" onClick={() => navigate("/shop/cart")}>View cart</button>
            </div>
            </>
        ) : (
            <>
            <span className="text-lg font-bold">Your cart is empty.</span>
            <div className="card-actions">
              <button className="btn btn-primary btn-block" onClick={() => navigate("/shop")}>Go to Shop</button>
            </div>
            </>
        )}
          </div>
        </div>
      </div>  
    )    
}