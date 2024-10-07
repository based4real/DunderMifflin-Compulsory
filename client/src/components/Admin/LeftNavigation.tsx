import { useLocation, useNavigate } from "react-router-dom";

export default function LeftNavigation() {
    const navigate = useNavigate()
    const location = useLocation(); 

    const isItemActive = (path: string) => {
        return location.pathname === path ? "active" : "";
    }

    return (
        <aside className="w-64 h-fit mt-12 shadow-md rounded-box lg:-ml-32">
        <ul className="menu menu-lg bg-base-100 border border-base-300 rounded-box w-auto h-fit">
            <li>
                <a className={`${isItemActive("/admin/orders")}`}>
                Orders
                </a>
            </li>
            <li>
                <a className={`${isItemActive("/admin/users")}`}>
                Users
                </a>
            </li>
            <li>
            <details open={location.pathname.startsWith("/admin/products")}>
                <summary>Products</summary>
                <ul>
                    <li>
                        <a className={`${isItemActive("/admin/products")}`}>
                            View all
                        </a>
                    </li>
                    <li>
                        <a className={`${isItemActive("/admin/products/create")}`}>
                        Create Product
                        </a>
                    </li>
                </ul>
            </details>
            </li>
            <li>
            <details open={location.pathname.startsWith("/admin/properties")}>
                <summary>Properties</summary>
                <ul>
                    <li>
                        <a className={`${isItemActive("/admin/properties")}`}>
                        View all
                        </a>
                    </li>
                    <li>
                        <a>
                            Assign Property
                        </a>
                    </li>
                    <li>
                        <a className={`${ location.pathname === "/admin/properties/create" ? "active" : "" }`}>
                            Create Property
                        </a>
                    </li>
                </ul>
            </details>
            </li>
            </ul>
        </aside>
    )
}