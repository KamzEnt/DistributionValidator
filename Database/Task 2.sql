--Task 2:
-- Identify clients that appear in the distribution output but have no corresponding approved transaction record.
SELECT
    c.client_id,
    c.client_name,
    d.distribution_id,
    t.status,
    t.transaction_date
FROM Distributions d
LEFT JOIN Clients c ON d.client_id = c.client_id
LEFT JOIN Transactions t ON d.distribution_id = t.distribution_id
WHERE t.status != "APPROVED"
OR t.transaction_id ISNULL;


-- Calculate the total distributed amount per month for the period covered by the data.
SELECT
    period,
    SUM(distributed_amount) AS total_distributed_amount
FROM Distributions
GROUP BY period
ORDER BY period;


-- Identify any clients whose distributed amount differs from their calculated source amount by more than $0.01.
SELECT 
    sc.client_id,
    c.client_name,
    sc.period,
    ROUND(sc.source_amount * (1 - sc.fee_pct), 2) AS expected_Amount,
    d.distributed_amount,
    ROUND(d.distributed_amount - (sc.source_amount * (1 - sc.fee_pct)), 2) as difference
FROM SourceCalculations sc
JOIN Distributions d ON sc.client_id = d.client_id AND sc.period = d.period
JOIN Clients c ON sc.client_id = c.client_id
WHERE ABS(d.distributed_amount - (sc.source_amount * (1 - sc.fee_pct))) > 0.01
ORDER BY sc.client_id, sc.period;