import { FilterType } from "../../Api";

interface FilterTypeSelectorProps {
    filterType: FilterType;
    onFilterTypeChange: (type: FilterType) => void;
}

export default function FilterTypeSelector({ filterType, onFilterTypeChange }: FilterTypeSelectorProps) {
    return (
        <div className="form-control mb-4">
            <h3 className="font-semibold mb-2">Filter Logic</h3>
            <div className="flex items-center justify-between">
                <label className="label cursor-pointer flex items-center">
                    <input
                        type="radio"
                        name="filterType"
                        className="radio radio-primary mr-2"
                        checked={filterType === FilterType.And}
                        onChange={() => onFilterTypeChange(FilterType.And)}
                    />
                    <span className="label-text">All</span>
                </label>
                <label className="label cursor-pointer flex items-center">
                    <input
                        type="radio"
                        name="filterType"
                        className="radio radio-primary mr-2"
                        checked={filterType === FilterType.Or}
                        onChange={() => onFilterTypeChange(FilterType.Or)}
                    />
                    <span className="label-text">Any</span>
                </label>
            </div>
        </div>
    );
}