import { useAtom } from "jotai";
import ShopCartProduct from "../components/Cart/ShopCartProduct";
import { CartAtom } from "../atoms/CartAtom";
import { CartOrderEntry } from "../model/CartModel";
import { useCallback } from "react";
import CartUpdater from "../components/Cart/CartUpdater";
import { api } from "../http";
import { useNavigate } from "react-router-dom";
import { OrderDetailViewModel } from "../Api";

export default function CartPage() {
    const [cart, setCart] = useAtom(CartAtom);
    const navigate = useNavigate();
    
    const placeOrder = () => {
        const orderEntries = cart.cartEntries.map((entry) => ({
            productId: entry.paper.id,
            quantity: entry.quantity,
        }));

        cart.customerId = 1;

        const orderData = {
            customerId: cart.customerId,
            orderEntries: orderEntries,
        };

        api.order.createOrder(orderData)
        .then((response) => {
          const order: OrderDetailViewModel = response.data;          
          navigate("/order/success", { state: { order } });
          
          setCart({ ...cart, cartEntries: [] });
          localStorage.removeItem('cartItems');
        })
        .catch(error => {
          console.error("Error placing order", error);
        });
    };

    const removeFromCart = useCallback((order: CartOrderEntry) => {
        const updatedCartEntries = cart.cartEntries.filter(
            (item) => item.paper.id !== order.paper.id
        );
        
        const updatedCart = {
            ...cart,
            cartEntries: updatedCartEntries,
        };
        
        setCart(updatedCart);
        localStorage.setItem('cartItems', JSON.stringify(updatedCart));
    }, [cart, setCart]);

    const updateQuantity = (order: CartOrderEntry, newQuantity: number) => {
        const updatedCartEntries = cart.cartEntries?.map((entry) =>
            entry.paper.id === order.paper.id
                ? { ...entry, quantity: newQuantity }
                : entry
        );

        const updatedCart = {
            ...cart,
            cartEntries: updatedCartEntries,
        };

        setCart(updatedCart);
        localStorage.setItem("cartItems", JSON.stringify(updatedCart));
    };

    const totalAmount = cart.cartEntries.reduce((total, entry) => {
        return total + (entry.paper.price * entry.quantity);
    }, 0) || 0;
    const totalFixed = totalAmount.toFixed(2);

    const totalQuantity = cart.cartEntries.reduce(
        (acc, entry) => acc + entry.quantity,
        0
      ) || 0;

    return (
    <>
        <CartUpdater />
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <main className="flex-1 pl-4 rounded-box">
            <div className="grid grid-cols-1 gap-3">
                {cart.cartEntries.map((entry, index) => (
                    <ShopCartProduct key={index} order={entry} removeFromCart={removeFromCart} updateQuantity={updateQuantity}/>
                ))}
                <div className="flex justify-center bg-base-100 rounded-box border border-base-300 p-4">
                {cart.cartEntries.length > 0 ? (
                    <h1 className="text-lg font-bold">Subtotal ({totalQuantity} items): <span className="font-bold"> ${totalFixed}</span></h1>
                ) : (
                    <h1 className="text-lg font-bold">Your cart is empty!</h1>
                )}
                </div>
            </div>
            <div className="flex justify-end">
                    
            </div>
            </main>
            {cart.cartEntries.length > 0 &&
            <aside className="w-64 bg-base-100 h-fit border border-base-300 p-4 shadow-md rounded-box">
                <h2 className="text-xl font-bold mb-4">Subtotal</h2>
                <div className="form-control gap-2">
                <span>{totalQuantity} Items: 
                    <span className="font-bold"> ${totalFixed}</span>
                </span>
                <div className="btn btn-primary" onClick={() => placeOrder()}>Buy now</div>
                </div>
            </aside>
            }
        </div>
        </div>
    </>
    )
}