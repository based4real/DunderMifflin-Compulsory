import { useLocation, useNavigate } from "react-router-dom";
import { OrderDetailViewModel } from "../Api";
import { useEffect } from "react";

export default function OrderPage() {
    const location = useLocation();
    const navigate = useNavigate();
    
    const order = location.state?.order as OrderDetailViewModel;

    useEffect(() => {
        if (!order) {
            navigate("/");
        }
    }, [order, navigate]);

    if (!order) return

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 h-fit max-w-screen-lg w-full justify-center">
            <div className="bg-base-100 border border-base-300 p-6 rounded-lg shadow-lg">
                <h1 className="text-2xl font-bold">Thank you for your order!</h1>
                <div className="divider"></div>
                We have successfully recieved your order #{order.id}
                <br/>
                <br/>
                <p>Status: {order.status || "Pending"}</p>
                <p>Order Date: {new Date(order.orderDate || "").toLocaleDateString()}</p>
                <p>Total Price: ${order.totalPrice.toFixed(2)}</p>

                <br/>
                Expected delivery is: {order.deliveryDate ? new Date(order.deliveryDate).toLocaleDateString() : "TBD"}
                <br/>
                You'll recieve an e-mail once your order is sent!
                <div className="divider"/>

                <h2 className="text-xl font-bold mt-4">Items:</h2>
                <ul className="list-disc list-inside">
                    {order.entry.map((item) => (
                        <li key={item.productId}>
                            {item.productName} (x{item.quantity})
                        </li>
                    ))}
                </ul>
            </div>
            </div>
        </div>
    );
}
