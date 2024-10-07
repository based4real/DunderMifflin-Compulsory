import { FaSearch, FaTimes } from "react-icons/fa";

interface ClearableSearchProps {
    searchTerm: string;
    onSearchChange: (value: string) => void;
    onClearSearch: () => void;
    promptText?: string;
}

export default function ClearableSearch({searchTerm, onSearchChange, onClearSearch, promptText = "Search..."}: ClearableSearchProps) {
    return (
        <div className="form-control w-full">
            <label className="input input-bordered flex items-center gap-2 relative">
                <input
                    type="text"
                    className="grow pr-10"
                    placeholder={promptText}
                    value={searchTerm}
                    onChange={(e) => onSearchChange(e.target.value)}
                />
                {searchTerm ? (
                    <button
                        type="button"
                        onClick={onClearSearch}
                        className="absolute right-2 text-gray-400 hover:text-gray-600 focus:outline-none"
                    >
                        <FaTimes />
                    </button>
                ) : (
                    <FaSearch className="absolute right-2 text-gray-400" />
                )}
            </label>
        </div>
    );
}