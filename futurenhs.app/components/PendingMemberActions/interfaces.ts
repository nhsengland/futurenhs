export interface Props {
    acceptAction: (formData: FormData) => Promise<Record<string, string>>
    rejectAction: (formData: FormData) => Promise<Record<string, string>>
    memberId: string
    text?: {
        acceptMember?: string
        rejectMember?: string
    }
}