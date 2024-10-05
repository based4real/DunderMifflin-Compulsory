import ShopCartProduct from "../components/Cart/ShopCartProduct";

export default function CartPage() {
    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <main className="flex-1 p-4 rounded-lg bg-base-300 ">
            <div className="grid grid-cols-1 gap-6">
            {Array.from({ length: 3 }, (_, index) => (
                <ShopCartProduct key={index}/>
            ))}
            </div>
            </main>
        </div>
        </div>
    )
}