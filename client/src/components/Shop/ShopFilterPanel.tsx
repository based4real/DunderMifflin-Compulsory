import DiscontinuedFilter from "../Filter/DiscontinuedFilter";
import FilterTypeSelector from "../Filter/FilterTypeSelector";
import PropertyFilter from "../Filter/PropertyFilter";
import PriceRangeFilter from "../Filter/PriceRangeFilter";
import { FilterType } from "../../Api"

interface ShopFilterPanelProps {
    discontinued: boolean | null;
    onDiscontinuedChange: (value: boolean | null) => void;
    filterType: FilterType;
    onFilterTypeChange: (type: FilterType) => void;
    selectedProperties: number[];
    onPropertiesChange: (selected: number[]) => void;
    minPrice: number | null;
    maxPrice: number | null;
    onPriceRangeChange: (min: number | null, max: number | null) => void;
}

export default function ShopFilterPanel({discontinued, 
                                         onDiscontinuedChange,
                                         filterType, 
                                         onFilterTypeChange, 
                                         selectedProperties, 
                                         onPropertiesChange, 
                                         minPrice, 
                                         maxPrice, 
                                         onPriceRangeChange}: ShopFilterPanelProps) {
    return (
        <div className="form-control mb-4">
            <h2 className="text-lg font-bold mb-4">Filters</h2>
            <DiscontinuedFilter discontinued={discontinued} onDiscontinuedChange={onDiscontinuedChange} />
            <FilterTypeSelector filterType={filterType} onFilterTypeChange={onFilterTypeChange} />
            <PropertyFilter selectedProperties={selectedProperties} onPropertiesChange={onPropertiesChange} />
            <PriceRangeFilter minPrice={minPrice} maxPrice={maxPrice} onPriceRangeChange={onPriceRangeChange} />
        </div>
    );
}