import { useAtom } from "jotai";
import ShopCartProduct from "../components/Cart/ShopCartProduct";
import { CartAtom } from "../atoms/CartAtom";
import { PaperDetailViewModel } from "../Api";
import { CartOrderEntry } from "../model/CartModel";
import { useCallback } from "react";

export default function CartPage() {
    const [cart, setCart] = useAtom(CartAtom);

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
                <div className="btn btn-primary">Buy now</div>
                </div>
            </aside>
            }
        </div>
        </div>
    )
}