import create from 'zustand'
import { persist } from 'zustand/middleware'
const useSessionStore = create(
    persist(
        (set, get) => ({
            session: null,
            ssr: null,
            setSession: (params) => {
                set(() => ({
                    session: params,
                }))
            },
        }),
        { name: 'session' }
    )
)

export default useSessionStore
