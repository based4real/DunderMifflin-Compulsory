import { OrderDetailViewModel } from "../../Api";

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
    </tr>
    )
}