import { FaEdit, FaEye, FaTrash } from "react-icons/fa";

export default function UserTableItem() {
    return (
        <tr>
        <td>
        <div>
              <div className="font-bold">Hart Hagerty</div>
              <div className="text-sm opacity-50">Sold: 2329</div>
            </div>
        </td>
        <td>
        Bulletproof
        <br />
        <div className="tooltip" data-tip="A4, Waterproof, Fireproof">
            <span className="badge badge-neutral cursor-pointer badge-sm">+5 others</span>
        </div>
        </td>
        <td>50</td>
        <td>$29.99</td>
        <th className="text-right">
        <div className="join join-vertical lg:join-horizontal">
            <button className="btn text-primary join-item"><FaEye/></button>
            <button className="btn text-primary join-item"><FaEdit/></button>
            <button className="btn text-primary join-item"><FaTrash/></button>
        </div>
        </th>
    </tr>
    )
}