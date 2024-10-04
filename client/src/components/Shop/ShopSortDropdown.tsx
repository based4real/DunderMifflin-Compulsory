import { ShopSortAtom } from "../../atoms/ShopSortAtom";
import { useAtom } from "jotai";


export default function ShopSortDropDown() {
    const [sort, setSort] = useAtom(ShopSortAtom);

  return (
    <div className="dropdown">
      <label tabIndex={0} className="btn m-1 bg-base-100">
        {sort}
      </label>
      <ul tabIndex={0} className="dropdown-content menu p-2 shadow bg-base-300 rounded-box w-52">
      <li>
          <a onClick={() => setSort("Newest")}>Newest</a>
        </li>
        <li>
          <a onClick={() => setSort("Price: Low to High")}>Price: Low to High</a>
        </li>
        <li>
          <a onClick={() => setSort("Price: High to Low")}>Price: High to Low</a>
        </li>
      </ul>
    </div>
  );
}
