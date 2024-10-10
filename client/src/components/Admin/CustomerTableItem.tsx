import { FaEdit, FaEye, FaTrash } from "react-icons/fa";
import { CustomerOrderDetailViewModel } from "../../Api";
import { useNavigate } from "react-router-dom";

export default function CustomerTableItem({ customer }: { customer: CustomerOrderDetailViewModel }) {
    const navigate = useNavigate();

    const handleViewClick = () => {
        navigate(`/admin/customers/${customer.id}`);
    };

    return (
        <tr>
        <td>
        <div>
              <div className="font-bold">{customer.name}</div>
              <div className="tooltip" data-tip={customer.email}>
                <div className="text-sm opacity-50 max-w-48 truncate">{customer.email}</div>
              </div>
            </div>
        </td>
        <td>
        {customer.phone}
        <br />
            <span className="badge badge-neutral cursor-pointer badge-sm">{customer.address}</span>
        </td>
        <td>{customer.totalOrders}</td>
        <th className="text-right">
        <div className="join join-vertical lg:join-horizontal">
            <button className="btn text-primary join-item" onClick={handleViewClick}><FaEye/></button>
        </div>
        </th>
    </tr>
    )
}