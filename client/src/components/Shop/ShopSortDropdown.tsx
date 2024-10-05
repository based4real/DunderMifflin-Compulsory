import { ShopSortAtom } from "../../atoms/ShopSortAtom";
import { useAtom } from "jotai";

export default function ShopSortDropDown() {
    const [sort, setSort] = useAtom(ShopSortAtom);

    const handleSortChange = (event: any) => {
        setSort(event.target.value);
    };

    return (
        <select className="select select-bordered select-md w-auto max-w-xs" value={sort} onChange={handleSortChange}>
            <option disabled value="">
                Sort by
            </option>
            <option value="newest">Newest</option>
            <option value="lowToHigh">Price: Low to High</option>
            <option value="highToLow">Price: High to Low</option>
        </select>
    );
}
