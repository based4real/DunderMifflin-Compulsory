import { FaPlus, FaSearch } from "react-icons/fa";
import LeftNavigation from "../../components/Admin/LeftNavigation";
import UserTableItem from "../../components/Admin/UserTableItem";
import { api } from "../../http";


export default function AdminUsersPage() {

    return (
        <div className="min-h-screen flex justify-center p-4 bg-base-200">
        <div className="flex flex-col lg:flex-row lg:space-x-6 max-w-screen-lg w-full">
            <LeftNavigation />
            <main className="flex-1 pl-4 pr-4 rounded-box">
            <div className="flex w-auto flex-row items-center">
                <div className="flex mt-2 justify-center">
                    <h3 className="font-bold text-2xl mb-2">Users</h3>
                </div>
            </div>
            <div className="form-control w-64 pb-2">
                <label className="input input-bordered flex items-center gap-2">
                <input
                    type="text"
                    className="grow"
                    placeholder="Search.."
                />
                <FaSearch/>
                </label>
            </div>
            <div className="grid grid-cols-1 gap-3">
            <div className="overflow-x-auto rounded-box border border-base-300 bg-base-100 p-0">
                <table className="table w-full">
                <thead>
                <tr>
                    <th>Name</th>
                    <th>Customer</th>
                    <th>Email</th>
                    <th>Phone</th>
                    <th>Orders</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {Array.from({ length: 5 }, (_, index) => (
                    <UserTableItem key={index}/>
                ))}
                </tbody>
            </table>
            </div>
            <div className="flex justify-center align-middle">
                <div className="flex join">
                <button className="join-item btn bg-base-100">1</button>
                <button className="join-item btn btn-active">2</button>
                <button className="join-item btn bg-base-100">3</button>
                <button className="join-item btn bg-base-100">4</button>
                </div>
            </div>
            </div>
            </main>
        </div>
        </div>
    )
}