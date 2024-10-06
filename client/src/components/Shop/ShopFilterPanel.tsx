import DiscontinuedFilter from "../Filter/DiscontinuedFilter";
import FilterTypeSelector from "../Filter/FilterTypeSelector";
import PropertyFilter from "../Filter/PropertyFilter";
import { FilterType } from "../../Api"

interface ShopFilterPanelProps {
    discontinued: boolean | null;
    onDiscontinuedChange: (value: boolean | null) => void;
    filterType: FilterType;
    onFilterTypeChange: (type: FilterType) => void;
    selectedProperties: number[];
    onPropertiesChange: (selected: number[]) => void;
}

export default function ShopFilterPanel({discontinued, 
                                         onDiscontinuedChange,
                                         filterType, 
                                         onFilterTypeChange, 
                                         selectedProperties, 
                                         onPropertiesChange}: ShopFilterPanelProps) {
    return (
        <div className="form-control mb-4">
            <h2 className="text-lg font-bold mb-4">Filters</h2>
            <DiscontinuedFilter discontinued={discontinued} onDiscontinuedChange={onDiscontinuedChange} />
            <FilterTypeSelector filterType={filterType} onFilterTypeChange={onFilterTypeChange} />
            <PropertyFilter selectedProperties={selectedProperties} onPropertiesChange={onPropertiesChange} />

            <div className="form-control mb-4">
                <h3 className="font-semibold mb-2">Price Range</h3>
                <label className="label cursor-pointer">
                    <input type="checkbox" className="checkbox checkbox-primary" />
                    <span className="label-text ml-2">$0 - $50</span>
                </label>

                <label className="label cursor-pointer">
                    <input type="checkbox" className="checkbox checkbox-primary" />
                    <span className="label-text ml-2">$50 - $100</span>
                </label>
            </div>
        </div>
    );
}