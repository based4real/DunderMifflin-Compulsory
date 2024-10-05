export default function LeftNavigation() {
    return (
        <aside className="w-64 h-fit mt-12 shadow-md rounded-box lg:-ml-32">
        <ul className="menu menu-lg bg-base-100 border border-base-300 rounded-box w-auto h-fit">
            <li>
                <a className="active">
                Dashboard
                </a>
            </li>
            <li>
                <a>
                Orders
                </a>
            </li>
            <li>
                <a>
                Users
                </a>
            </li>
            <li>
                <details>
                <summary>Products</summary>
                <ul>
                    <li><a>View all</a></li>
                    <li><a>Create Product</a></li>
                </ul>
                </details>
            </li>
            <li>
                <details>
                <summary>Properties</summary>
                <ul>
                    <li><a>View all</a></li>
                    <li><a>Create Property</a></li>
                </ul>
                </details>
            </li>
            </ul>
        </aside>
    )
}