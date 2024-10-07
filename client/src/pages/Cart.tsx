import ShopCartProduct from "../components/Cart/ShopCartProduct";

export default function CartPage() {
    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <main className="flex-1 pl-4 rounded-box">
            <div className="grid grid-cols-1 gap-3">
                {Array.from({ length: 3 }, (_, index) => (
                    <ShopCartProduct key={index}/>
                ))}
                <div className="flex justify-center bg-base-100 rounded-box border border-base-300 p-4">
                    <h1 className="text-lg font-bold">Subtotal (3 items): <span className="font-bold"> 493$</span></h1>

                </div>
            </div>
            <div className="flex justify-end">
                    
            </div>
            </main>
            <aside className="w-64 bg-base-100 h-fit border border-base-300 p-4 shadow-md rounded-box">
                <h2 className="text-xl font-bold mb-4">Subtotal</h2>
                <div className="form-control gap-2">
                <span>2 Items: 
                    <span className="font-bold"> 493$</span>
                </span>
                <div className="btn btn-primary">Buy now</div>
                </div>
            </aside>
        </div>
        </div>
    )
}