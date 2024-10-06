import { useState, useEffect } from "react";

interface DiscontinuedFilterProps {
    discontinued: boolean | null;
    onDiscontinuedChange: (value: boolean | null) => void;
}

export default function DiscontinuedFilter({ discontinued, onDiscontinuedChange }: DiscontinuedFilterProps) {
    const [currentState, setCurrentState] = useState<boolean | null>(discontinued);

    useEffect(() => {
        setCurrentState(discontinued);
    }, [discontinued]);

    const handleDropdownChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const value = event.target.value;
        let newState: boolean | null = null;

        if (value === "available") newState = false;
        else if (value === "discontinued") newState = true;

        onDiscontinuedChange(newState);
    };

    return (
        <div className="form-control mb-4">
            <select
                className="select select-bordered w-full max-w-xs"
                value={currentState === null ? "all" : currentState ? "discontinued" : "available"}
                onChange={handleDropdownChange}>
                
                <option value="all">Show All</option>
                <option value="available">Show Available</option>
                <option value="discontinued">Show Discontinued</option>
            </select>
        </div>
    );
}