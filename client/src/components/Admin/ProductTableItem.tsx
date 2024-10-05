export default function ProductTableItem() {
    return (
        <tr>
        <th>
        <label>
            <input type="checkbox" className="checkbox" />
        </label>
        </th>
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
        <th>
        <button className="btn btn-ghost btn-xs">details</button>
        </th>
    </tr>
    )
}