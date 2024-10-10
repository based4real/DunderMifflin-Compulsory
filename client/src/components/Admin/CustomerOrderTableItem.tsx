import { OrderDetailViewModel } from "../../Api";
import {FaEdit} from "react-icons/fa";

export default function CustomerOrderTableItem({ order }: { order: OrderDetailViewModel }) {
    return (
        <tr>
        <td>
            <div className="font-bold">{order.id}</div>
        </td>
        <td>
            {new Date(order.orderDate).toLocaleDateString()} {new Date(order.orderDate).toLocaleTimeString()}
        </td>
        <td>
            {order.deliveryDate}
        </td>
        <td>
            {order.entry.length}
        </td>
        <td>
            ${order.totalPrice}
        </td>
        <td>
            {order.status}
        </td>
        <td>
            <button className="btn text-primary join-item">
                <FaEdit />
            </button>
        </td>
    </tr>
    )
}