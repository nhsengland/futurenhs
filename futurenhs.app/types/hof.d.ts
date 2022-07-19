import { GetServerSidePropsResult } from 'next'
import { GetServerSidePropsContext } from '@appTypes/next'

export interface HofConfig {
    isRequired?: boolean;
}

export type Hof = (
    context: GetServerSidePropsContext,
    config?: HofConfig,
    dependencies?: Record<string, any>
) => Promise<any>