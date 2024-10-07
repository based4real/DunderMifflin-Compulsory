import { ShopSortAtom } from "../../atoms/ShopAtoms";
import { useAtom } from "jotai";
import { PaperOrderBy, SortOrder } from "../../Api";

interface SortOption {
    label: string;
    value: string;
    orderBy: PaperOrderBy;
    sortBy: SortOrder;
}

const sortOptions: SortOption[] = [
    { label: "Newest", value: "idDesc", orderBy: PaperOrderBy.Id, sortBy: SortOrder.Desc },
    { label: "Oldest", value: "idAsc", orderBy: PaperOrderBy.Id, sortBy: SortOrder.Asc },
    { label: "Name: A-Z", value: "nameAsc", orderBy: PaperOrderBy.Name, sortBy: SortOrder.Asc },
    { label: "Name: Z-A", value: "nameDesc", orderBy: PaperOrderBy.Name, sortBy: SortOrder.Desc },
    { label: "Price: Low to High", value: "priceAsc", orderBy: PaperOrderBy.Price, sortBy: SortOrder.Asc },
    { label: "Price: High to Low", value: "priceDesc", orderBy: PaperOrderBy.Price, sortBy: SortOrder.Desc },
    { label: "Stock: Low to High", value: "stockAsc", orderBy: PaperOrderBy.Stock, sortBy: SortOrder.Asc },
    { label: "Stock: High to Low", value: "stockDesc", orderBy: PaperOrderBy.Stock, sortBy: SortOrder.Desc }
];

export default function ShopSortDropDown() {
    const [sort, setSort] = useAtom(ShopSortAtom);
    
    const currentValue = sortOptions.find(
        option => option.orderBy === sort.orderBy && option.sortBy === sort.sortBy
    )?.value ?? "";

    const handleSortChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const value = event.target.value;
        const selectedOption = sortOptions.find(option => option.value === value);

        if (selectedOption) {
            setSort({
                orderBy: selectedOption.orderBy,
                sortBy: selectedOption.sortBy
            });
        }
    };

    return (
        <select
            className="select select-bordered select-md w-auto max-w-xs"
            value={currentValue}
            onChange={handleSortChange}>
            
            <option disabled value="">
                Sort by
            </option>
            
            {sortOptions.map((option) => (
                <option key={option.value} value={option.value}>
                    {option.label}
                </option>
            ))}
        </select>
    );
}